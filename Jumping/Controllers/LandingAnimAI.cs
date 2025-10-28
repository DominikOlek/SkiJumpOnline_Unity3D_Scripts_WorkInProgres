using Assets.Scripts.Animation;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts.Jumping.Controllers
{
    [RequireComponent(typeof(ArmatureAnim))]
    [System.Serializable]
    public class LandingAnimAI : ACLandingAnim
    {
        private void Update()
        {
            updateTick(simulateMouse(axis));
        }

        private void LateUpdate()
        {
            lateUpdateTick();
        }

        private float simulateMouse(float axisToSim)
        {
            float missFloat = -2f + (jumperInfo.controlFactorDown * 3);
            float factor_move = Random.Range(-0.25f / missFloat, 1f * missFloat);
            return -(axisToSim == 0 ? 0 : Mathf.Sign(axisToSim) * Mathf.Pow(Mathf.Abs(axisToSim), 1.2f)) * factor_move;
        }
    }

    //public class LandingAnimAI : MonoBehaviour, PointAnimI, ILandingAnim
    //{
    //    [Header("Settings")]
    //    [SerializeField] SkiJumpInfo jumpInfo;
    //    [SerializeField] JumperInfoGlobal jumperInfo;

    //    ArmatureAnim armature;
    //    Transform bodyMain, back;

    //    public float Factor { get; set; } = 1;
    //    [SerializeField] float axis = 0;
    //    [SerializeField] float speed = 0;
    //    [SerializeField] Slider axisSlider;

    //    float avgFactor = 5;
    //    int frame = 1;
    //    float mouseXCtrl;

    //    float timer = 0.3f;

    //    private void OnEnable()
    //    {
    //        timer = 0.3f;
    //        avgFactor = 0;
    //        frame = 0;
    //        Factor = 1;
    //        axis = 0;
    //        speed = 0;
    //    }


    //    private void Awake()
    //    {
    //        armature = gameObject.GetComponent<ArmatureAnim>();
    //        bodyMain = armature.bodyMain;
    //        back = armature.back;
    //    }

    //    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //    void Start()
    //    {
    //        speed = Random.Range(0.2f, 0.24f) * Mathf.Sign(Random.Range(-1, 1));
    //        avgFactor = 0;
    //        frame = 0;
    //        mouseXCtrl = jumpInfo.controlValue * jumperInfo.mouseXSens;
    //    }

    //    private void OnDisable()
    //    {
    //        avgFactor /= frame;
    //        axisSlider.value = 0;
    //    }
    //    // Update is called once per frame
    //    void Update()
    //    {
    //        axis += speed * Time.deltaTime * 10;
    //        if (Mathf.Abs(axis) < 5)
    //            speed += (speed / 150 * Mathf.Max(axis / jumpInfo.divisionSwing, 1)) * Time.deltaTime + simulateMouse(axis) * mouseXCtrl;
    //        else
    //            speed += simulateMouse(axis) * mouseXCtrl;

    //        if (Random.Range(0f, 1f) < jumpInfo.swingFrequencyOther / (0.5f + jumperInfo.bootsFactor/2) / jumperInfo.controlFactorDown)
    //        {
    //            speed += Random.Range(0.1f, 0.2f) / jumperInfo.swingsFactorDown * Mathf.Sign(Random.Range(-1, 1)) / (0.5f + jumperInfo.bootsFactor / 2);
    //        }

    //        axis = Mathf.Sign(axis) * Mathf.Min(5, Mathf.Abs(axis));
    //        speed = Mathf.Sign(speed) * Mathf.Min(1f, Mathf.Abs(speed));

    //        Factor = Mathf.Max(1 - Mathf.Abs(axis) / 5, 0.35f);
    //        axisSlider.value = axis;
    //        avgFactor += Mathf.Abs(axis);
    //        frame += 1;

    //        if (axis >= 5f)
    //        {
    //            timer -= Time.deltaTime;
    //            if (timer <= 0)
    //            {
    //                armature.animCtrl.RunCrashRight();
    //                avgFactor = 5f;
    //                frame = 1;
    //                this.enabled = false;
    //            }
    //        }
    //        else if (axis <= -5f)
    //        {
    //            timer -= Time.deltaTime;
    //            if (timer <= 0)
    //            {
    //                armature.animCtrl.RunCrashLeft();
    //                avgFactor = 5f;
    //                frame = 1;
    //                this.enabled = false;
    //            }
    //        }
    //        else
    //        {
    //            timer = 0.3f;
    //        }
    //    }

    //    void LateUpdate()
    //    {
    //        setAnim(axis);
    //    }

    //    private float simulateMouse(float axisToSim)
    //    {
    //        float missFloat = -2f + (jumperInfo.controlFactorDown * 3);
    //        float factor_move = Random.Range(-0.25f / missFloat, 1f * missFloat);
    //        return -(axisToSim == 0 ? 0 : Mathf.Sign(axisToSim) * Mathf.Pow(Mathf.Abs(axisToSim), 1.2f)) * factor_move;
    //    }

    //    public void setAnim(float axis)
    //    {
    //        bodyMain.Rotate(-axis * 4.1f, 0, Mathf.Abs(axis) * -2.1f);
    //        back.Rotate(axis * 2.1f, 0, Mathf.Abs(axis) * -2.1f);
    //    }
    //    public float GetPoint()
    //    {
    //        return (5 - avgFactor) / 5;
    //    }

    //    public float getAxis()
    //    {
    //        return axis;
    //    }

    //    public void setEnable(bool status)
    //    {
    //        this.enabled = status;
    //    }
    //}
}