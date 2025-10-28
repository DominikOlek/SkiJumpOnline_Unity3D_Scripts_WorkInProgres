using Assets.Scripts.Animation;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts.Jumping.Controllers
{
    [RequireComponent(typeof(ArmatureAnim))]
    [System.Serializable]
    public class InRunAnimAI : ACInRunAnim
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
            float missFloat = -2f + (jumperInfo.controlFactorRun * 3);
            float factor_move = Random.Range(-0.25f / missFloat, 1f * missFloat);
            return -(axisToSim == 0 ? 0 : Mathf.Sign(axisToSim) * Mathf.Pow(Mathf.Abs(axisToSim), 1.2f)) * factor_move;
        }
    }

    //public class InRunAnimAI : MonoBehaviour, PointAnimI, IInRunAnim
    //{
    //    [Header("Settings")]
    //    [SerializeField] SkiJumpInfo jumpInfo;
    //    [SerializeField] JumperInfoGlobal jumperInfo;

    //    ArmatureAnim armature;
    //    Transform bodyMain, armLeft, armRight, backLeft, backRight;

    //    public float Factor { get; set; } = 1;
    //    [SerializeField] float axis = 0;
    //    [SerializeField] float speed = 0;
    //    [SerializeField] Slider axisSlider;

    //    float avgFactor = 5;
    //    int frame = 1;
    //    float mouseXCtrl;

    //    private void OnEnable()
    //    {
    //        avgFactor = 0;
    //        frame = 0;
    //        Factor = 1;
    //        axis = 0;
    //        speed = Random.Range(0.2f, 0.24f) * Mathf.Sign(Random.Range(-1, 1));
    //    }

    //    private void Awake()
    //    {
    //        armature = gameObject.GetComponent<ArmatureAnim>();
    //        bodyMain = armature.bodyMain;
    //        backLeft = armature.backLeft;
    //        backRight = armature.backRight;
    //        armRight = armature.armRight;
    //        armLeft = armature.armLeft;
    //    }
    //    private void Start()
    //    {
    //        avgFactor = 0;
    //        frame = 0;
    //        mouseXCtrl = jumpInfo.controlValue * jumperInfo.mouseXSens;
    //    }
    //    private void OnDisable()
    //    {
    //        avgFactor /= frame;
    //    }

    //    private void Update()
    //    {
    //        axis += speed * Time.deltaTime * 10;
    //        if (Mathf.Abs(axis) < 5)
    //            speed += (speed / 150 * Mathf.Max(axis / jumpInfo.divisionSwing, 1)) * Time.deltaTime + simulateMouse(axis) * mouseXCtrl;
    //        else
    //            speed += simulateMouse(axis) * mouseXCtrl;

    //        if (Random.Range(0f, 1f) < jumpInfo.swingFrequencyOther / jumperInfo.controlFactorRun)
    //        {
    //            speed += Random.Range(0.1f, 0.2f) / jumperInfo.swingsFactorRun * Mathf.Sign(Random.Range(-1, 1));
    //        }

    //        axis = Mathf.Sign(axis) * Mathf.Min(5, Mathf.Abs(axis));
    //        speed = Mathf.Sign(speed) * Mathf.Min(1f, Mathf.Abs(speed));

    //        Factor = Mathf.Max(1 - Mathf.Abs(axis) / 5, 0.35f) * (0.5f + jumperInfo.lubricationFactor/2);
    //        axisSlider.value = axis;

    //        avgFactor += Mathf.Abs(axis);
    //        frame += 1;
    //    }

    //    private float simulateMouse(float axisToSim)
    //    {
    //        float missFloat = -2f+(jumperInfo.controlFactorRun * 3); 
    //        float factor_move = Random.Range(-0.25f / missFloat, 1f * missFloat);
    //        return -(axisToSim == 0 ? 0 : Mathf.Sign(axisToSim) * Mathf.Pow(Mathf.Abs(axisToSim), 1.2f)) * factor_move;
    //    }

    //    void LateUpdate()
    //    {
    //        setAnim(axis);
    //        //bodyMain.rotation = new Quaternion(axis + bodyMain.rotation.x, bodyMain.rotation.y, bodyMain.rotation.z, bodyMain.rotation.w);
    //    }

    //    public float GetPoint()
    //    {
    //        return (5 - avgFactor) / 5;
    //    }

    //    public void setAnim(float axis)
    //    {
    //        bodyMain.Rotate(-axis * 5f, -axis * 3f, 0);
    //        backLeft.Rotate(0, 0, -axis * 3f);
    //        backRight.Rotate(0, 0, -axis * 3f);
    //        armLeft.Rotate(0, 0, -axis * 2f);
    //        armRight.Rotate(0, 0, -axis * 2f);
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