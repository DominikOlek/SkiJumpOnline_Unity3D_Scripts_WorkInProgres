using Assets.Scripts.Animation;
using Assets.Scripts.Audio;
using Assets.Scripts.Competition.Other;
using Assets.Scripts.Jumping.StaticInfo;
using System.Linq;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Assets.Scripts.Jumping.Controllers
{
    [RequireComponent(typeof(FitToGorund))]
    [RequireComponent(typeof(JumperInfo))]
    [RequireComponent(typeof(IFlyKeyboardController))]
    [System.Serializable]
    public class Fly : MonoBehaviour, PointAnimI
    {
        [Header("Settings")]
        SceneObjects sceneObjects;
        SkiJumpInfoGlobal jumpInfo;
        [SerializeField] JumperInfoGlobal jumperInfo;
        [SerializeField] IFlyKeyboardController keyboardController;

        private float resistance = 0.001f;

        public float speed = 0;

        Vector3 velocity;
        private CinemachineSplineCart cart, cartGate;
        SplineAutoDolly.FixedSpeed SplineAutoDollyf;
        FitToGorund fitToGround;

        [SerializeField] AnimationControler animCtrl;

        private AudioController audioController;

        [Header("Anim")]
        [SerializeField] GameObject animChild;
        ACInRunAnim animInRun;
        ACFlyAnim animFly;
        ACLandingAnim animLand;
        [SerializeField] Points pointsCnt;

        LayerMask downHill;

        float pointLandingStyle = 0.01f;

        public int state { get; set; } = 0;

        float dist = 0;
        Vector3 lastP = Vector3.zero;
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        private void Awake()
        {
            GameObject tmp = GameObject.FindGameObjectsWithTag("SceneData").FirstOrDefault();
            if (tmp == null)
            {
                Debug.LogError("Not Found SceneData");
                return;
            }
            sceneObjects = tmp.GetComponent<SceneObjects>();
            jumpInfo = sceneObjects.skijumpInfo;
            animInRun = animChild.GetComponent<ACInRunAnim>();
            animFly = animChild.GetComponent<ACFlyAnim>();
            animLand = animChild.GetComponent<ACLandingAnim>();
            keyboardController = GetComponent<IFlyKeyboardController>();
            fitToGround = GetComponent<FitToGorund>();
            cartGate = GameObject.FindWithTag("Gate").GetComponent<CinemachineSplineCart>();
            cart = GetComponent<CinemachineSplineCart>();

            tmp = GameObject.FindGameObjectsWithTag("AudioController").FirstOrDefault();
            if (tmp == null)
            {
                Debug.LogError("Not Found AudioController");
                return;
            }
            audioController = tmp.GetComponent<AudioController>();
        }

        private void OnEnable()
        {
            pointLandingStyle = 0f;
            state = 0;
            dist = 0;
            lastP = Vector3.zero;
            speed = 0;

            fitToGround.enabled = false;
            SplineAutoDollyf = cart.AutomaticDolly.Method as SplineAutoDolly.FixedSpeed;
            SplineAutoDollyf.Speed = 0;
            startAnim();
            animInRun.setEnable(false);
            animFly.setEnable(false);
            animLand.setEnable(false);
            cart.SplinePosition = jumpInfo.maxPositionValue + (jumpInfo.onePoisitionDif * (jumpInfo.maxPosition - jumpInfo.startPosition));
            cartGate.SplinePosition = cart.SplinePosition;
            //cart.SplinePosition = jumpInfo.startPositionValue;
            cart.enabled = true;
            keyboardController.resetState();

            audioController.PlayStart();
        }

        public void startAnim()
        {
            animCtrl.RunIdle();
        }

        private void OnDisable()
        {
            fitToGround.enabled = false;
        }
        void Start()
        {
            downHill = LayerMask.NameToLayer("DownHill");
            resistance = jumpInfo.resistance;
        }

        // Update is called once per frame
        void Update()
        {
            switch (state)
            {
                case 0:
                    {
                        if (keyboardController.isStart())
                        {
                            audioController.PlayInrun();
                            state = 1;
                            animInRun.setEnable(true);
                            animCtrl.RunStart();
                        }
                        break;
                    }
                case 1:
                    {
                        sceneObjects.setActiveExtraInfo(true);
                        sceneObjects.ExtraInfoTraining.text = "SPEED: " + (speed * 2.9f).ToString("0.0");
                        if (speed < jumpInfo.maxSpeed)
                        {
                            speed += jumpInfo.increaseSpeed * Time.deltaTime * animInRun.Factor;
                            SplineAutoDollyf.Speed = speed;
                        }
                        if (keyboardController.isJump())
                        {
                            Up();
                            float timing = Mathf.Abs(jumpInfo.bestUp - cart.SplinePosition);
                            if (timing / jumpInfo.lostHeightFactor < 1)
                            {
                                velocity = new Vector3(0, Mathf.Max(1.5f - timing / jumpInfo.lostHeightFactor, 0), 0);
                                transform.position += velocity * Time.deltaTime * jumperInfo.bootsFactor;
                                speed -= (timing / jumpInfo.lostHeightFactor * 2) / jumperInfo.bootsFactor;
                            }
                            else
                            {
                                speed -= 5;
                            }

                        }
                        else if (cart.SplinePosition >= cart.Spline.CalculateLength() * 0.99)
                        {
                            cart.enabled = false;
                            speed *= jumpInfo.lostSpeedFactorLate;
                            resistance *= jumpInfo.multiplyResistanceLate;
                            state = 4;
                        }
                        break;
                    }
                case 2:
                    {
                        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 5f, LayerMask.GetMask("Default", "Hill", "TransparentFX")))
                        {
                            if (hit.distance < 0.001f && hit.transform.gameObject.layer == LayerMask.NameToLayer("TransparentFX"))
                            {
                                animCtrl.RunDown(Landing.Crash);
                                pointLandingStyle = hit.distance / 3;
                                state = 3;
                                audioController.PlayCrash();
                                fitToGround.Fit(speed, hit);
                                animFly.setEnable(false);
                            }
                            else if (hit.distance <= jumpInfo.crashHeight && hit.transform.gameObject.layer != LayerMask.NameToLayer("TransparentFX"))
                            {
                                animCtrl.RunDown(Landing.Crash);
                                pointLandingStyle = hit.distance / 3;
                                state = 3;
                                audioController.PlayCrash();
                                animFly.setEnable(false);
                            }
                            setDistance(hit);
                        }

                        velocity += Physics.gravity * Time.deltaTime * animFly.FactorHeight;
                        setFly(animFly.FactorSpeed);

                        (bool, bool) mouseTuple = keyboardController.isLanding();
                        if (mouseTuple.Item1 || mouseTuple.Item2)
                        {
                            if (Physics.Raycast(transform.position, -transform.up, out hit, 30f, LayerMask.GetMask("Hill")))
                            {
                                float distance = hit.distance;
                                switch (distance)
                                {
                                    case float i when i < jumpInfo.badHeight:
                                        {
                                            if (mouseTuple.Item1 || mouseTuple.Item2)
                                            {
                                                animCtrl.RunDown(Landing.Crash);
                                                pointLandingStyle = hit.distance / 3;
                                            }
                                            break;
                                        }
                                    case float i when i < jumpInfo.OkHeight:
                                        {
                                            if (mouseTuple.Item1 || mouseTuple.Item2)
                                            {
                                                animCtrl.RunDown(Landing.WithTouch);
                                                pointLandingStyle = 1.5f + hit.distance / 3;
                                                animLand.setEnable(true);
                                            }
                                            break;
                                        }
                                    case float i when i < jumpInfo.NiceHeight || hit.transform.tag == "Flat":
                                        {
                                            if (mouseTuple.Item1 && mouseTuple.Item2)
                                            {
                                                animCtrl.RunDown(Landing.TwoLegs);
                                                pointLandingStyle = Mathf.Min(3.7f + hit.distance / 3, 6);
                                            }
                                            else
                                            {
                                                animCtrl.RunDown(Landing.WithTouch);
                                                pointLandingStyle = 1.5f + hit.distance / 3;
                                            }
                                            animLand.setEnable(true);
                                            break;
                                        }
                                    default:
                                        {
                                            if (mouseTuple.Item1 && mouseTuple.Item2)
                                            {
                                                animCtrl.RunDown(Landing.TwoLegs);
                                                pointLandingStyle = Mathf.Min(3.7f + hit.distance / 3, 6);
                                            }
                                            else
                                            {
                                                animCtrl.RunDown(Landing.Telemark);
                                                pointLandingStyle = Mathf.Min(6f + hit.distance / 3, 7);
                                            }
                                            animLand.setEnable(true);
                                            break;
                                        }
                                }

                            }
                            state = 3;
                            animFly.setEnable(false);
                        }
                        break;
                    }
                case 3:
                    {
                        sceneObjects.setActiveExtraInfo(false);

                        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 0.75f, LayerMask.GetMask("Default", "Hill", "DownHill")))
                        {
                            fitToGround.Fit(speed, hit);
                            if (speed > 2f) animLand.StartDust();
                            audioController.PlayLand();

                            if (hit.transform.gameObject.layer == downHill)
                            {
                                speed -= jumpInfo.resistanceDown * Time.deltaTime * (0.9f+(Mathf.Pow(-speed+50,3)/250000));
                            }
                            else
                            {
                                speed -= jumpInfo.resistanceHill * Time.deltaTime;
                            }
                            audioController.ChangeVolume(speed/40);
                        }
                        else
                        {
                            velocity += Physics.gravity * Time.deltaTime*2.5f;
                            setFly();

                            if (Physics.Raycast(transform.position, -transform.up, out hit, 20f, LayerMask.GetMask("Hill")))
                            {
                                if (hit.distance > 1.6f)
                                {
                                    setDistance(hit);
                                }
                            }
                        }

                        if (speed < 2f)
                        {
                            speed = 0;
                            animLand.setEnable(false);
                            pointsCnt.enabled = true;
                            audioController.StopLand();
                        }
                        break;
                    }
                case 4:
                    {
                        if (keyboardController.isJump())
                        {
                            resistance /= jumpInfo.multiplyResistanceLate;
                            Up();
                        }
                        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, jumpInfo.badHeight, LayerMask.GetMask("Default", "Hill")))
                        {
                            animCtrl.RunDown(Landing.Crash);
                            pointLandingStyle = hit.distance / 3;
                            state = 3;
                            animFly.setEnable(false);
                            animInRun.setEnable(false);
                        }


                        velocity += Physics.gravity * Time.deltaTime*2;
                        setFly();
                        break;
                    }
            }

        }

        private void setFly(float boost = 1)
        {
            velocity.x = speed;
            speed -= resistance * boost;
            transform.position += velocity * Time.deltaTime;
        }

        private void Up()
        {
            audioController.PlayJump();
            animCtrl.RunUp();
            animFly.setEnable(true);
            animInRun.setEnable(false);
            state = 2;
            fitToGround.enabled = true;
            cart.enabled = false;
        }

        private void setDistance(RaycastHit hit)
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Hill"))
            {
                if (lastP != Vector3.zero) dist += Vector3.Distance(lastP, hit.point);
                lastP = hit.point;
            }
        }

        public float getDistance()
        {
            return dist - dist % 0.05f;
        }


        public float GetPoint()
        {
            return pointLandingStyle / 7;
        }
    }
}