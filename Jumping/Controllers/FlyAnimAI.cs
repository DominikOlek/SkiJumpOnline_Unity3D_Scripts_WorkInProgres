using Assets.Scripts.Animation;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts.Jumping.Controllers
{
    [RequireComponent(typeof(ArmatureAnim))]
    [System.Serializable]

    public class FlyAnimAI : ACFlyAnim
    {
        private void Update()
        {
            updateTick(simulateMouse(axisSide), simulateMouse(axis), simulateMouse(axis) * 4);
        }

        private float simulateMouse(float axisToSim)
        {
            float missFloat = -2 + (jumperInfo.controlFactorFly * 3); // 0,4 - 1,6 liniowa
            float factor_move = Random.Range(-0.25f / missFloat, 1f * missFloat);
            return (axisToSim == 0 ? 0 : Mathf.Sign(axisToSim) * Mathf.Pow(Mathf.Abs(axisToSim), 1.2f)) * factor_move;
        }

        private void LateUpdate()
        {
            lateUpdateTick();
        }
    }

    //public class FlyAnimAI : MonoBehaviour, PointAnimI, IFlyAnim
    //{
    //    [Header("Settings")]
    //    [SerializeField] SkiJumpInfo jumpInfo;
    //    [SerializeField] JumperInfoGlobal jumperInfo;

    //    ArmatureAnim armature;
    //    Transform bodyMain, backLeft, backRight, legLeft, legRight, main;

    //    public float FactorSpeed { get; set; } = 1;
    //    public float FactorHeight { get; set; } = 1;
    //    [SerializeField] float axis { get; set; } = 0;
    //    [SerializeField] float speed = 0;

    //    [SerializeField] float axisSide { get; set; } = 0;
    //    [SerializeField] float speedSide = 0;
    //    public bool isFly = false;

    //    [SerializeField] WindController windCtrl;

    //    [SerializeField] Slider axisSlider, axisSideSlider;
    //    float timer = 0.4f;

    //    float avgFactor = 5;
    //    int frame = 1;

    //    float mouseYCtrl, mouseXCtrl;

    //    private void OnEnable()
    //    {
    //        isFly = false;
    //        timer = 0.4f;

    //        FactorHeight = 1;
    //        FactorSpeed = 1;
    //        axis = 0;
    //        axisSide = 0;
    //        speedSide = 0;

    //        speed = -Random.Range(0.7f, 0.9f);
    //        windCtrl.managePointCnt(true);
    //        avgFactor = 0;
    //        frame = 0;
    //    }

    //    private void Awake()
    //    {
    //        armature = gameObject.GetComponent<ArmatureAnim>();
    //        bodyMain = armature.bodyMain;
    //        backLeft = armature.backLeft;
    //        backRight = armature.backRight;
    //        legLeft = armature.legLeft;
    //        legRight = armature.legRight;
    //        main = armature.main;
    //    }

    //    private void Start()
    //    {
    //        mouseYCtrl = jumpInfo.controlValue * jumperInfo.mouseYSens;
    //        mouseXCtrl = jumpInfo.controlValue * jumperInfo.mouseXSens;
    //    }

    //    private void OnDisable()
    //    {
    //        axisSlider.value = 0;
    //        avgFactor /= frame;
    //        windCtrl.managePointCnt(false);
    //    }

    //    private void Update()
    //    {
    //        if (!isFly)
    //        {
    //            axis += speed * Time.deltaTime*10f;
    //            speed -= Time.deltaTime*12f;
    //            speed += 4*simulateMouse(axis) * -(jumpInfo.controlValue / 10);

    //            axis = Mathf.Min(0, axis);
    //            axis = Mathf.Max(-5, axis);
    //            speed = Mathf.Sign(speed) * Mathf.Min(2.5f, Mathf.Abs(speed));

    //            FactorSpeed = 0.7f + (Mathf.Abs(axis) / 20);
    //            FactorHeight = 0.85f + (Mathf.Abs(axis) / 30);
    //        }
    //        else
    //        {
    //            axis += speed * Time.deltaTime * 10;
    //            if (Mathf.Abs(axis) < 5)
    //                speed += (speed / 300 * Mathf.Max(axis / jumpInfo.divisionSwing, 1)) * Time.deltaTime + simulateMouse(axis) * -mouseYCtrl;
    //            else
    //                speed += simulateMouse(axis) * -mouseYCtrl;

    //            if (Random.Range(0f, 1f) < jumpInfo.swingFrequencyFly / jumperInfo.controlFactorFly)
    //            {
    //                speed += Random.Range(Mathf.Abs(windCtrl.getSpeed() / jumpInfo.divisionWindSwingMIN), Mathf.Abs(windCtrl.getSpeed() / jumpInfo.divisionWindSwingMAX)) / jumperInfo.swingsFactorFly * -Mathf.Sign(windCtrl.getSpeed());
    //            }

    //            axis = Mathf.Sign(axis) * Mathf.Min(5, Mathf.Abs(axis));
    //            speed = Mathf.Sign(speed) * Mathf.Min(1f, Mathf.Abs(speed));



    //            axisSide += speedSide * Time.deltaTime * 10;
    //            if (Mathf.Abs(axis) < 5)
    //                speedSide += (speed / 300 * Mathf.Max(axis / jumpInfo.divisionSwing, 1)) * Time.deltaTime + simulateMouse(axisSide) * -mouseXCtrl;
    //            else
    //                speedSide += simulateMouse(axisSide) * -mouseXCtrl;

    //            if (Random.Range(0f, 1f) < jumpInfo.swingFrequencyFly / jumperInfo.controlFactorFly)
    //            {
    //                speedSide += Random.Range(Mathf.Abs(windCtrl.getDir() / jumpInfo.divisionWindSwingMIN), Mathf.Abs(windCtrl.getDir() / jumpInfo.divisionWindSwingMAX)) / jumperInfo.swingsFactorFly * -Mathf.Sign(windCtrl.getDir());
    //            }

    //            axisSide = Mathf.Sign(axisSide) * Mathf.Min(5, Mathf.Abs(axisSide));
    //            speedSide = Mathf.Sign(speedSide) * Mathf.Min(1f, Mathf.Abs(speedSide));

    //            FactorSpeed = 0.75f + (axis / 20) / (0.5f + jumperInfo.suitFactor / 2);

    //            if (axis < 0)
    //                FactorHeight = 0.85f + (Mathf.Abs(axis) / 15);
    //            else
    //                FactorHeight = 0.85f + (Mathf.Abs(axis) / 25);

    //            FactorHeight += ((Mathf.Abs(axisSide) / 25) + windCtrl.getSpeed() / 25) / (0.5f + jumperInfo.suitFactor / 2);

    //        }

    //        if (Mathf.Abs(axis) >= 5f || Mathf.Abs(axisSide) >= 5f)
    //        {
    //            timer -= Time.deltaTime;
    //            if (timer <= 0)
    //            {
    //                armature.animCtrl.RunDown(Landing.Crash);
    //                FactorSpeed = 2f;
    //                FactorHeight = 5f;
    //                this.enabled = false;
    //            }
    //        }
    //        else
    //        {
    //            timer = 0.4f;
    //        }

    //        axisSideSlider.value = -axisSide;
    //        axisSlider.value = -axis;

    //        avgFactor += Mathf.Abs(axis);
    //        frame += 1;
    //    }

    //    void LateUpdate()
    //    {
    //        setAnim(axisSide, axis);
    //        //bodyMain.rotation = new Quaternion(axis + bodyMain.rotation.x, bodyMain.rotation.y, bodyMain.rotation.z, bodyMain.rotation.w);
    //    }

    //    private float simulateMouse(float axisToSim)
    //    {
    //        float missFloat = -2 + (jumperInfo.controlFactorFly * 3); // 0,4 - 1,6 liniowa
    //        float factor_move = Random.Range(-0.25f / missFloat, 1f * missFloat); 
    //        return (axisToSim == 0 ? 0 : Mathf.Sign(axisToSim) * Mathf.Pow(Mathf.Abs(axisToSim), 1.2f)) * factor_move;
    //    }

    //    public void EndUp()
    //    {
    //        isFly = true;
    //        speed = Random.Range(Mathf.Abs(windCtrl.getSpeed() / 6), Mathf.Abs(windCtrl.getSpeed() / 3)) * -Mathf.Sign(windCtrl.getSpeed());
    //        speedSide = Random.Range(Mathf.Abs(windCtrl.getDir() / 6), Mathf.Abs(windCtrl.getDir() / 3)) * -Mathf.Sign(windCtrl.getDir());
    //    }

    //    public float GetPoint()
    //    {
    //        return (5 - avgFactor) / 5;
    //    }

    //    public void setAnim(float axisSide, float axis)
    //    {
    //        main.Rotate(0, axisSide * 2f, axis * 3f);
    //        bodyMain.Rotate(0, axisSide * 2f, axis * 3f);
    //        backLeft.Rotate(0, axisSide * 2f, axis * 3f);
    //        backRight.Rotate(0, axisSide * 2f, axis * 3f);
    //    }
    //    public float getAxisSpeed()
    //    {
    //        return axis;
    //    }

    //    public float getAxisSide()
    //    {
    //        return axisSide;
    //    }

    //    public void setEnable(bool status)
    //    {
    //        this.enabled = status;
    //    }
    //}
}