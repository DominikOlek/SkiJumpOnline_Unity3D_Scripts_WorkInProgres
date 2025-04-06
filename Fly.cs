using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(FitToGorund))]
[RequireComponent(typeof(JumperInfo))]
[System.Serializable]
public class Fly : MonoBehaviour, PointAnimI
{
    [Header("Settings")]
    [SerializeField] SkiJumpInfo jumpInfo;
    [SerializeField] JumperInfo jumperInfo;

    [Header("InRun")]
    [SerializeField] float maxSpeed;
    [SerializeField] float runIncrease = 0.001f;

    [Header("Up")]
    [SerializeField] float perfectUp;
    [SerializeField] float lostHeightFactor;
    [SerializeField] float lostSpeedFactorLate;
    [SerializeField] float multiplyResistanceLate;

    [Header("Fly")]
    [SerializeField] float resistance = 0.001f;

    public float speed = 0;

    Vector3 velocity;
    private CinemachineSplineCart cart;
    SplineAutoDolly.FixedSpeed SplineAutoDollyf;
    FitToGorund fitToGround;

    [SerializeField] AnimationControler animCtrl;

    [Header("Anim")]
    [SerializeField] GameObject animChild;
    InRunAnim animInRun;
    FlyAnim animFly;
    LandingAnim animLand;
    [SerializeField] Points pointsCnt;

    LayerMask downHill;

    float pointLandingStyle = 0f;

    public int state { get; set; } = 0;

    float dist = 0;
    Vector3 lastP = Vector3.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        animInRun = animChild.GetComponent<InRunAnim>();
        animFly = animChild.GetComponent<FlyAnim>();
        animLand = animChild.GetComponent<LandingAnim>();
        fitToGround = GetComponent<FitToGorund>();

        cart = GetComponent<CinemachineSplineCart>();

        pointLandingStyle = 0f;
        state = 0;
        dist = 0;
        lastP = Vector3.zero;
        speed = 0;

        fitToGround.enabled = false;
        SplineAutoDollyf = cart.AutomaticDolly.Method as SplineAutoDolly.FixedSpeed;
        SplineAutoDollyf.Speed = 0;
        startAnim();
        animInRun.enabled = false;
        animFly.enabled = false;
        animLand.enabled = false;
        cart.SplinePosition = jumpInfo.startPositionValue;
        cart.enabled = true;
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
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case 0:
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        state = 1;
                        animInRun.enabled = true;
                        animCtrl.RunStart();
                    }
                    break;
                }
            case 1:
                {
                    if (speed < maxSpeed)
                    {
                        speed += runIncrease * Time.deltaTime * animInRun.Factor;
                        SplineAutoDollyf.Speed = speed;
                    }
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        Up();
                        float timing = Mathf.Abs(perfectUp - cart.SplinePosition);
                        if (timing / lostHeightFactor < 1)
                        {
                            velocity = new Vector3(0, Mathf.Max(1f - timing / lostHeightFactor, 0), 0);
                            transform.position += velocity * Time.deltaTime;
                            speed -=(timing/ lostHeightFactor*2);
                        }
                        else
                        {
                            speed -= 1;
                        }

                    } else if (cart.SplinePosition >= cart.Spline.CalculateLength()*0.99) {
                        cart.enabled = false;
                        speed *= lostSpeedFactorLate;
                        resistance *= multiplyResistanceLate;
                        state = 4;
                    }
                    break;
                }
            case 2: {
                    if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 20f, LayerMask.GetMask("Default","Hill")))
                    {
                        if (hit.distance <= jumpInfo.crashHeight)
                        {
                            animCtrl.RunDown(Landing.Crash);
                            pointLandingStyle = hit.distance / 3;
                            state = 3;
                            animFly.enabled = false;
                        }
                        setDistance(hit);
                    }

                    velocity += Physics.gravity * Time.deltaTime * animFly.FactorHeight;
                    setFly(animFly.FactorSpeed);


                    if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1))
                    {
                        if (Physics.Raycast(transform.position, -transform.up, out hit, 10f, LayerMask.GetMask("Hill")))
                        {
                            float distance = hit.distance;
                            switch (distance)
                            {
                                case float i when i < jumpInfo.badHeight:
                                    {
                                        if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1))
                                        {
                                            animCtrl.RunDown(Landing.Crash);
                                            pointLandingStyle = hit.distance / 3;
                                        }
                                        break;
                                    }
                                case float i when i < jumpInfo.OkHeight:
                                    {
                                        if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1))
                                        {
                                            animCtrl.RunDown(Landing.Bad);
                                            pointLandingStyle = 1.5f + hit.distance / 3;
                                            animLand.enabled = true;
                                        }
                                        break;
                                    }
                                case float i when i < jumpInfo.NiceHeight:
                                    {
                                        if (Input.GetKey(KeyCode.Mouse0) && Input.GetKey(KeyCode.Mouse1))
                                        {
                                            animCtrl.RunDown(Landing.Ok);
                                            pointLandingStyle = Mathf.Min(3.7f + hit.distance / 3, 6);
                                        }
                                        else
                                        {
                                            animCtrl.RunDown(Landing.Bad);
                                            pointLandingStyle = 1.5f + hit.distance / 3;
                                        }
                                        animLand.enabled = true;
                                        break;
                                    }
                                default:
                                    {
                                        if (Input.GetKey(KeyCode.Mouse0) && Input.GetKey(KeyCode.Mouse1))
                                        {
                                            animCtrl.RunDown(Landing.Ok);
                                            pointLandingStyle = Mathf.Min(3.7f + hit.distance / 3, 6);
                                        }
                                        else
                                        {
                                            animCtrl.RunDown(Landing.Nice);
                                            pointLandingStyle = Mathf.Min(6f + hit.distance / 3, 7);
                                        }
                                        animLand.enabled = true;
                                        break;
                                    }
                            }
                            
                        }
                        state = 3;
                        animFly.enabled = false;
                    }
                    break;
                }
            case 3: {
                    if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 0.75f))
                    {
                        fitToGround.Fit(speed, hit);

                        if (hit.transform.gameObject.layer == downHill)
                        {
                            speed -= resistance * 5600 * Time.deltaTime;
                        }
                        else
                        {
                            speed -= resistance * 200 * Time.deltaTime;
                        }
                    }
                    else
                    {
                        velocity += Physics.gravity * Time.deltaTime;
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
                        animLand.enabled = false;
                        pointsCnt.enabled = true;
                    }
                    break;
                }
            case 4: {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        resistance /= multiplyResistanceLate;
                        Up();
                    }
                    if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, jumpInfo.badHeight, LayerMask.GetMask("Default", "Hill")))
                    {
                        animCtrl.RunDown(Landing.Crash);
                        pointLandingStyle = hit.distance / 3;
                        state = 3;
                        animFly.enabled = false;
                        animInRun.enabled = false;
                    }


                    velocity += Physics.gravity * Time.deltaTime;
                    setFly();
                    break;
                }
        }

    }

    private void setFly(float boost=1) {
        velocity.x = speed;
        speed -= resistance * boost;
        transform.position += velocity * Time.deltaTime;
    }

    private void Up()
    {
        animCtrl.RunUp();
        animFly.enabled= true;
        animInRun.enabled = false;
        state = 2;
        fitToGround.enabled = true;
        cart.enabled = false;
    }

    private void setDistance(RaycastHit hit) {
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Hill"))
        {
            if (lastP != Vector3.zero) dist += Vector3.Distance(lastP, hit.point);
            lastP = hit.point;
        }
    }

    public float getDistance() {     
        return dist - dist%0.5f; 
    }


    public float GetPoint()
    {
        return pointLandingStyle/7;
    }
}
