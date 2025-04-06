using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ArmatureAnim))]
[System.Serializable]
public class FlyAnim : MonoBehaviour , PointAnimI
{
    [Header("Settings")]
    [SerializeField] SkiJumpInfo jumpInfo;
    [SerializeField] JumperInfo jumperInfo;

    ArmatureAnim armature;
    Transform bodyMain, backLeft, backRight,legLeft,legRight,main;

    public float FactorSpeed { get; set; } = 1;
    public float FactorHeight { get; set; } = 1;
    [SerializeField] float axis { get; set; } = 0;
    [SerializeField] float speed = 0;

    [SerializeField] float axisSide { get; set; } = 0;
    [SerializeField] float speedSide = 0;
    bool isFly = false;

    [SerializeField] WindController windCtrl;

    [SerializeField] Slider axisSlider,axisSideSlider;
    float timer = 0.4f;

    float avgFactor = 5;
    int frame = 1;

    float mouseYCtrl,mouseXCtrl;

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
        avgFactor = 0;
        frame = 0;
    }

    private void Awake()
    {
        armature = gameObject.GetComponent<ArmatureAnim>();
        bodyMain = armature.bodyMain;
        backLeft = armature.backLeft;
        backRight = armature.backRight;
        legLeft = armature.legLeft;
        legRight = armature.legRight;
        main = armature.main;
    }

    private void Start()
    {
        mouseYCtrl = jumpInfo.controlValue * jumperInfo.ControlFactorFly * jumperInfo.MouseYSens;
        mouseXCtrl = jumpInfo.controlValue * jumperInfo.ControlFactorFly * jumperInfo.MouseXSens;
    }

    private void OnDisable()
    {
        avgFactor /= frame;
        windCtrl.managePointCnt(false);
    }

    private void Update()
    {
        if (!isFly)
        {
            axis += speed * Time.deltaTime * 10;
            speed -= 12f * Time.deltaTime;
            speed += Input.GetAxis("Mouse Y") * -(jumpInfo.controlValue/10);

            axis = Mathf.Min(0, axis);
            axis = Mathf.Max(-5, axis);
            speed = Mathf.Sign(speed) * Mathf.Min(2.5f, Mathf.Abs(speed));

            FactorSpeed = 0.75f + (Mathf.Abs(axis) / 20);
            FactorHeight = 0.85f + (Mathf.Abs(axis) / 30);
        }
        else
        {
            axis += speed * Time.deltaTime * 15;
            if (Mathf.Abs(axis) < 5)
                speed += (speed / 300 * Mathf.Max(axis/jumpInfo.divisionSwing, 1)) * Time.deltaTime + Input.GetAxis("Mouse Y") * -mouseYCtrl;
            else
                speed += Input.GetAxis("Mouse Y") * -mouseYCtrl;

            if (Random.Range(0f, 1f) < jumpInfo.swingFrequencyFly)
            {
                speed += Random.Range(Mathf.Abs(windCtrl.getSpeed() / jumpInfo.divisionWindSwingMIN), Mathf.Abs(windCtrl.getSpeed() / jumpInfo.divisionWindSwingMAX)) *jumperInfo.SwingsFactorFly * -Mathf.Sign(windCtrl.getSpeed());
            }

            axis = Mathf.Sign(axis) * Mathf.Min(5, Mathf.Abs(axis));
            speed = Mathf.Sign(speed) * Mathf.Min(0.5f, Mathf.Abs(speed));



            axisSide += speedSide * Time.deltaTime * 10;
            if (Mathf.Abs(axis) < 5)
                speedSide += (speed / 300 * Mathf.Max(axis/ jumpInfo.divisionSwing, 1)) * Time.deltaTime + Input.GetAxis("Mouse X") * -mouseXCtrl;
            else
                speedSide += Input.GetAxis("Mouse X") * -mouseXCtrl;

            if (Random.Range(0f, 1f) < jumpInfo.swingFrequencyFly)
            {
                speedSide += Random.Range(Mathf.Abs(windCtrl.getDir() / jumpInfo.divisionWindSwingMIN), Mathf.Abs(windCtrl.getDir() / jumpInfo.divisionWindSwingMAX)) * -Mathf.Sign(windCtrl.getDir());
            }

            axisSide = Mathf.Sign(axisSide) * Mathf.Min(5, Mathf.Abs(axisSide));
            speedSide = Mathf.Sign(speedSide) * Mathf.Min(1f, Mathf.Abs(speedSide));

            FactorSpeed = 0.75f + (axis / 20);

            if (axis < 0)
                FactorHeight = 0.85f + (Mathf.Abs(axis) / 15);
            else
                FactorHeight = 0.85f + (Mathf.Abs(axis) / 25);

            FactorHeight += ((Mathf.Abs(axisSide) / 25) + windCtrl.getSpeed()/24)*jumperInfo.suitFactor;

        }

        if (Mathf.Abs(axis) >= 5f || Mathf.Abs(axisSide) >= 5f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                armature.animCtrl.RunDown(Landing.Crash);
                FactorSpeed = 2f;
                FactorHeight = 5f;
                this.enabled = false;
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

    void LateUpdate()
    {
        setAnim(axisSide, axis);
        //bodyMain.rotation = new Quaternion(axis + bodyMain.rotation.x, bodyMain.rotation.y, bodyMain.rotation.z, bodyMain.rotation.w);
    }

    public void EndUp() {
        isFly = true;
        speed = Random.Range(Mathf.Abs(windCtrl.getSpeed()/6), Mathf.Abs(windCtrl.getSpeed()/3)) * -Mathf.Sign(windCtrl.getSpeed());
        speedSide = Random.Range(Mathf.Abs(windCtrl.getDir()/6), Mathf.Abs(windCtrl.getDir()/3)) * -Mathf.Sign(windCtrl.getDir());
    }

    public float GetPoint()
    {
        return (5 - avgFactor) / 5;
    }

    public void setAnim(float axisSide, float axis)
    {
        Debug.Log(main+"to tu");
        main.Rotate(0, axisSide * 2f, axis * 3f);
        bodyMain.Rotate(0, axisSide * 2f, axis * 3f);
        backLeft.Rotate(0, axisSide * 2f, axis * 3f);
        backRight.Rotate(0, axisSide * 2f, axis * 3f);
    }
    public float getAxisSpeed()
    {
        return axis;
    }

    public float getAxisSide()
    {
        return axisSide;
    }
}
