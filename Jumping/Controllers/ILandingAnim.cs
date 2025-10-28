using Assets.Scripts.Animation;
using Assets.Scripts.Competition.Other;
using Assets.Scripts.Jumping.StaticInfo;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//public interface ILandingAnim
//{
//    float Factor { get; set; }

//    float getAxis();
//    float GetPoint();
//    void setAnim(float axis);
//    void setEnable(bool status);
//}
namespace Assets.Scripts.Jumping.Controllers
{
    public abstract class ACLandingAnim : MonoBehaviour, PointAnimI, IGetAxis
    {
        [Header("Settings")]
        SceneObjects sceneObjects;
        SkiJumpInfoGlobal jumpInfo;
        [SerializeField] protected JumperInfoGlobal jumperInfo;
        [SerializeField] protected Settings settings;

        ArmatureAnim armature;
        Transform bodyMain, back , legLeft, legRight, armLeft, armRight;
        [SerializeField] ParticleController particleController;

        public float Factor { get; set; } = 1;
        [SerializeField] protected float axis = 0;
        [SerializeField] float speed = 0;
        [SerializeField] Slider axisSlider;

        float avgFactor = 0.01f;
        int frame = 1;
        protected float mouseXCtrl;

        float timer = 0.3f;

        private void OnEnable()
        {
            timer = 0.3f;
            avgFactor = 0.01f;
            frame = 1;
            Factor = 1;
            axis = 0;
            speed = 0;
        }

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
            armature = gameObject.GetComponent<ArmatureAnim>();
            bodyMain = armature.bodyMain;
            back = armature.back;
            armRight = armature.armRight;
            armLeft = armature.armLeft;
            legLeft = armature.legLeft;
            legRight = armature.legRight;
        }

        void Start()
        {
            speed = Random.Range(0.2f, 0.24f) * Mathf.Sign(Random.Range(-1, 1));
            avgFactor = 0;
            frame = 0;
            SetSens();
        }

        protected virtual void SetSens()
        {
            mouseXCtrl = jumpInfo.controlValue;
        }
        public void StartDust()
        {
            if (jumperInfo.landingStyle != Landing.Crash)
            {
                particleController.PlayAfterLand();
            }
            else
            {
                particleController.PlayCrash();
            }
        }

        private void OnDisable()
        {
            avgFactor /= frame;
            axisSlider.value = 0;
        }

        protected void updateTick(float axisX)
        {
            axis += speed * Time.deltaTime * 10;
            if (Mathf.Abs(axis) < 5)
                speed += (speed / 150 * Mathf.Max(axis / jumpInfo.divisionSwing, 1)) * Time.deltaTime + axisX * mouseXCtrl;
            else
                speed += axisX * mouseXCtrl;

            if (Random.Range(0f, 1f) < jumpInfo.swingFrequencyOther / (0.5f + jumperInfo.bootsFactor / 2) / jumperInfo.controlFactorDown)
            {
                speed += Random.Range(0.2f, 0.3f) / jumperInfo.swingsFactorDown * Mathf.Sign(Random.Range(-1, 1)) / (0.5f + jumperInfo.bootsFactor / 2);
            }

            axis = Mathf.Sign(axis) * Mathf.Min(5, Mathf.Abs(axis));
            speed = Mathf.Sign(speed) * Mathf.Min(1f, Mathf.Abs(speed));

            Factor = Mathf.Max(1 - Mathf.Abs(axis) / 5, 0.35f);
            axisSlider.value = axis;
            avgFactor += Mathf.Abs(axis);
            frame += 1;

            if (axis >= 5f)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    armature.animCtrl.RunCrashRight();
                    avgFactor = 5f;
                    frame = 1;
                    this.enabled = false;
                }
            }
            else if (axis <= -5f)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    armature.animCtrl.RunCrashLeft();
                    avgFactor = 5f;
                    frame = 1;
                    this.enabled = false;
                }
            }
            else
            {
                timer = 0.3f;
            }
        }

        protected void lateUpdateTick()
        {
            setAnim(axis);
        }

        public void setAnim(float axis)
        {
            bodyMain.Rotate(-axis * 4.1f, 0, Mathf.Abs(axis) * -2.1f);
            back.Rotate(axis * 2.1f, 0, Mathf.Abs(axis) * -2.1f);
            armLeft.Rotate(0, 0, -axis * 2f);
            armRight.Rotate(0, 0, -axis * 2f);
            legRight.Rotate(0, -axis*0.7f, 0);
            legLeft.Rotate(0, axis * 0.7f, 0);
        }
        public float GetPoint()
        {
            return (5 - avgFactor) / 5;
        }

        public void setEnable(bool status)
        {
            this.enabled = status;
        }

        /// <summary>
        /// returns frame to check if it crash, when frame = 1
        /// </summary>
        /// <returns></returns>
        public (float, float) getAxis()
        {
            return (axis,frame);
        }
    }
}