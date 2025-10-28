using UnityEngine;

namespace Assets.Scripts.Jumping.StaticInfo
{
    public class JumperInfo : MonoBehaviour
    {
        public ScriptableObject jumper;
        [Header("InRun")]
        public float controlFactorRun = 1;
        public float swingsFactorRun = 1;
        public float lubricationFactor = 1;

        [Header("Fly")]
        public float controlFactorFly = 1;
        public float swingsFactorFly = 1;
        public float suitFactor = 1;

        [Header("Down")]
        public float controlFactorDown = 1;
        public float swingsFactorDown = 1;
        public float bootsFactor = 1;


        [Header("Settings")]
        public float mouseXSens = 1;
        public float mouseYSens = 1;

        public JumpState jumpState;

        [Header("Others")]
        public float money = 1.0f;
    }

    public enum JumpState
    {
        Idle, InRun, Fly, Down
    }
}