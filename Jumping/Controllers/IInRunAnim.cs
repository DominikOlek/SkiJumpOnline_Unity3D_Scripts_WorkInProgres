using Assets.Scripts.Animation;
using Assets.Scripts.Audio;
using Assets.Scripts.Competition.Other;
using Assets.Scripts.Jumping.StaticInfo;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Jumping.Controllers
{
    public interface IInRunAnim
    {
        float Factor { get; set; }
        float GetPoint();
        void setAnim(float axis);
        public void setEnable(bool status);
    }

    public abstract class ACInRunAnim : MonoBehaviour, PointAnimI, IGetAxis
    {
        [Header("Settings")]
        SceneObjects sceneObjects;
        SkiJumpInfoGlobal jumpInfo;
        [SerializeField] protected JumperInfoGlobal jumperInfo;
        [SerializeField] protected Settings settings;

        ArmatureAnim armature;
        Transform bodyMain, armLeft, armRight, backLeft, backRight, legLeft, legRight;
        float animFlag = 0;
        [SerializeField] ParticleController particleController;
        [SerializeField] AudioController audioController;

        public float Factor { get; set; } = 1;
        [SerializeField] protected float axis = 0;
        [SerializeField] float speed = 0;
        [SerializeField] Slider axisSlider;

        float avgFactor = 0.01f;
        int frame = 1;
        protected float mouseXCtrl;

        private void OnEnable()
        {
            avgFactor = 0.01f;
            frame = 1;
            Factor = 1;
            axis = 0;
            speed = Random.Range(0.2f, 0.24f) * Mathf.Sign(Random.Range(-1, 1));
            animFlag = 0;
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
            backLeft = armature.backLeft;
            backRight = armature.backRight;
            armRight = armature.armRight;
            armLeft = armature.armLeft;
            legLeft = armature.legLeft;
            legRight = armature.legRight;
        }
        private void Start()
        {
            avgFactor = 0;
            frame = 0;

            SetSens();
        }

        protected virtual void SetSens()
        {
            mouseXCtrl = jumpInfo.controlValue;
        }
        private void OnDisable()
        {
            avgFactor /= frame;
            particleController.StopInRunHit();
            audioController.StopInRunHits();
        }

        protected void updateTick(float axisX)
        {
            axis += speed * Time.deltaTime * 10;
            if (Mathf.Abs(axis) < 5)
                speed += (speed / 150 * Mathf.Max(axis / jumpInfo.divisionSwing, 1)) * Time.deltaTime + axisX * mouseXCtrl;
            else
                speed += axisX * mouseXCtrl;


            if (Random.Range(0f, 1f) < jumpInfo.swingFrequencyOther / jumperInfo.controlFactorRun)
            {
                speed += Random.Range(0.1f, 0.2f) / jumperInfo.swingsFactorRun * Mathf.Sign(Random.Range(-1, 1));
            }

            axis = Mathf.Sign(axis) * Mathf.Min(5, Mathf.Abs(axis));
            speed = Mathf.Sign(speed) * Mathf.Min(1f, Mathf.Abs(speed));

            Factor = Mathf.Max(1 - Mathf.Abs(axis) / 5, 0.35f) * (0.5f + jumperInfo.lubricationFactor / 2);
            axisSlider.value = axis;

            avgFactor += Mathf.Abs(axis);
            frame += 1;
        }

        protected void lateUpdateTick()
        {
            setAnim(axis);
        }

        public float GetPoint()
        {
            return (5 - avgFactor) / 5;
        }

        public void setAnim(float axis)
        {
            bodyMain.Rotate(-axis * 5f, -axis * 3f, 0);
            backLeft.Rotate(0, 0, -axis * 3f);
            backRight.Rotate(0, 0, -axis * 3f);
            armLeft.Rotate(0, 0, -axis * 2f);
            armRight.Rotate(0, 0, -axis * 2f);
            legRight.Rotate(0, -axis*0.7f, 0);
            legLeft.Rotate(0, -axis * 0.7f, 0);
            if(Mathf.Abs(axis) > 3)
            {
                audioController.PlayInRunHits();
                animFlag += Time.deltaTime;
            }else
                audioController.StopInRunHits();
            if (animFlag >= 0.4f)
            {
                legRight.Rotate(0, -axis * (0.45f + Mathf.Abs(0.025f*(animFlag-0.55f))), 0);
                legLeft.Rotate(0, -axis * (0.45f + Mathf.Abs(0.025f * (animFlag - 0.55f))), 0);
                particleController.PlayInRunHit();
                if (animFlag > 0.7f)
                {
                    animFlag = 0;
                    particleController.StopInRunHit();
                }
            }
        }

        public void setEnable(bool status)
        {
            this.enabled = status;
        }

        public (float, float) getAxis()
        {
            return (axis, 0);
        }
    }
}