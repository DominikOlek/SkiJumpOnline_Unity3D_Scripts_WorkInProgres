using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Jumping.StaticInfo;
using System.Linq;
using Assets.Scripts.Animation;
using Assets.Scripts.Competition.Other;

namespace Assets.Scripts.Jumping.Controllers
{
    //public interface IFlyAnim
    //{
    //    float FactorHeight { get; set; }
    //    float FactorSpeed { get; set; }

    //    void EndUp();
    //    float getAxisSide();
    //    float getAxisSpeed();
    //    float GetPoint();
    //    void setAnim(float axisSide, float axis);
    //    void setEnable(bool status);
    //}

    public abstract class ACFlyAnim : MonoBehaviour, PointAnimI
    {
        [Header("Settings")]
        SceneObjects sceneObjects;
        SkiJumpInfoGlobal jumpInfo;
        [SerializeField] protected JumperInfoGlobal jumperInfo;
        [SerializeField] protected Settings settings;

        ArmatureAnim armature;
        Transform bodyMain, backLeft, backRight, legLeft, legRight, main, armLeft,armRight;

        public float FactorSpeed { get; set; } = 1;
        public float FactorHeight { get; set; } = 1;
        [SerializeField] protected float axis { get; set; } = 0;
        [SerializeField] float speed = 0;

        [SerializeField] protected float axisSide { get; set; } = 0;
        [SerializeField] float speedSide = 0;
        bool isFly = false;

        [SerializeField] WindController windCtrl;

        [SerializeField] Slider axisSlider, axisSideSlider;
        float timer = 0.4f;

        float avgFactor = 0.01f;
        int frame = 1;

        protected float mouseYCtrl, mouseXCtrl;

        private void OnEnable()
        {
            isFly = false;
            timer = 0.4f;

            FactorHeight = 1;
            FactorSpeed = 1;
            axis = 0;
            axisSide = 0;
            speedSide = 0;

            speed = -Random.Range(0.7f, 0.9f);
            windCtrl.managePointCnt(true);
            avgFactor = 0.01f;
            frame = 1;
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
            legLeft = armature.legLeft;
            legRight = armature.legRight;
            armLeft = armature.armLeft;
            armRight = armature.armRight;
            main = armature.main;
        }

        private void Start()
        {
            SetSens();
        }

        protected virtual void SetSens()
        {
            mouseYCtrl = jumpInfo.controlValue;
            mouseXCtrl = jumpInfo.controlValue;
        }

        private void OnDisable()
        {
            avgFactor /= frame;
            windCtrl.managePointCnt(false);
        }

        protected void updateTick(float axisX, float axisY, float axisYInJump)
        {
            if (!isFly)
            {
                axis += speed * Time.deltaTime * 10;
                speed -= 12f * Time.deltaTime;
                speed += axisYInJump * -(jumpInfo.controlValue / 10);

                axis = Mathf.Min(0, axis);
                axis = Mathf.Max(-5, axis);
                speed = Mathf.Sign(speed) * Mathf.Min(2.5f, Mathf.Abs(speed));

                FactorSpeed = 0.7f + (Mathf.Abs(axis) / 20);
                FactorHeight = 0.85f + (Mathf.Abs(axis) / 30);
            }
            else
            {
                axis += speed * Time.deltaTime * 10;
                if (Mathf.Abs(axis) < 5)
                    speed += (speed / 300 * Mathf.Max(axis / jumpInfo.divisionSwing, 1)) * Time.deltaTime + axisY * -mouseYCtrl;
                else
                    speed += axisY * -mouseYCtrl;

                if (Random.Range(0f, 1f) < jumpInfo.swingFrequencyFly / jumperInfo.controlFactorFly)
                {
                    speed += Random.Range(Mathf.Abs(windCtrl.getSpeed() / jumpInfo.divisionWindSwingMIN), Mathf.Abs(windCtrl.getSpeed() / jumpInfo.divisionWindSwingMAX)) / jumperInfo.swingsFactorFly * -Mathf.Sign(windCtrl.getSpeed());
                }

                axis = Mathf.Sign(axis) * Mathf.Min(5, Mathf.Abs(axis));
                speed = Mathf.Sign(speed) * Mathf.Min(1f, Mathf.Abs(speed));



                axisSide += speedSide * Time.deltaTime * 10;
                if (Mathf.Abs(axis) < 5)
                    speedSide += (speed / 300 * Mathf.Max(axis / jumpInfo.divisionSwing, 1)) * Time.deltaTime + axisX * -mouseXCtrl;
                else
                    speedSide += axisX * -mouseXCtrl;

                if (Random.Range(0f, 1f) < jumpInfo.swingFrequencyFly / jumperInfo.controlFactorFly)
                {
                    speedSide += Random.Range(Mathf.Abs(windCtrl.getDir() / jumpInfo.divisionWindSwingMIN), Mathf.Abs(windCtrl.getDir() / jumpInfo.divisionWindSwingMAX)) / jumperInfo.swingsFactorFly * -Mathf.Sign(windCtrl.getDir());
                }
                axisSide = Mathf.Sign(axisSide) * Mathf.Min(5, Mathf.Abs(axisSide));
                speedSide = Mathf.Sign(speedSide) * Mathf.Min(1f, Mathf.Abs(speedSide));

                FactorSpeed = (0.75f + (axis / 20)) / (0.5f + jumperInfo.suitFactor / 2);

                if (axis < 0)
                    FactorHeight = 0.85f + (Mathf.Abs(axis) / 15);
                else
                    FactorHeight = 0.85f + (Mathf.Abs(axis) / 25);

                FactorHeight += ((Mathf.Abs(axisSide) / 25) + windCtrl.getSpeed() / 25) / (0.5f + jumperInfo.suitFactor / 2);

            }

            if (Mathf.Abs(axis) >= 5f || Mathf.Abs(axisSide) >= 5f)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    armature.animCtrl.RunDown(Landing.Crash);
                    FactorSpeed = 2f;
                    FactorHeight = 5f;
                    setEnable(false);
                }
            }
            else
            {
                timer = 0.4f;
            }

            axisSideSlider.value = -axisSide;
            axisSlider.value = -axis;

            avgFactor += Mathf.Abs(axis);
            frame += 1;
        }


        public void EndUp()
        {
            isFly = true;
            speed = Random.Range(Mathf.Abs(windCtrl.getSpeed() / 6), Mathf.Abs(windCtrl.getSpeed() / 3)) * -Mathf.Sign(windCtrl.getSpeed());
            speedSide = Random.Range(Mathf.Abs(windCtrl.getDir() / 6), Mathf.Abs(windCtrl.getDir() / 3)) * -Mathf.Sign(windCtrl.getDir());
        }

        public float GetPoint()
        {
            return (5 - avgFactor) / 5;
        }

        public void setAnim(float axisSide, float axis)
        {
            main.Rotate(0, axisSide * 2f, axis * 3f);
            bodyMain.Rotate(0, axisSide * 2f, axis * 3f);
            backLeft.Rotate(0, axisSide * 2f, axis * 3f);
            backRight.Rotate(0, axisSide * 2f, axis * 3f);
            armLeft.Rotate(0, axisSide * 2f, axis * 2f);
            armRight.Rotate(0, axisSide * 2f, axis * 2f);
            legLeft.Rotate(0, axisSide * 2f, axis * 2f);
            legRight.Rotate(0, axisSide * 2f, axis * 2f);
        }
        public (float, float) getAxis()
        {
            return (axisSide, axis);
        }

        public void setEnable(bool status)
        {
            this.enabled = status;
        }

        protected void lateUpdateTick()
        {
            setAnim(axisSide, axis);
        }
    }
}