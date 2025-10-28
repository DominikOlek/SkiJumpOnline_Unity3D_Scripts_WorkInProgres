using Assets.Scripts.Jumping.Controllers;
using UnityEngine;

namespace Assets.Scripts.Jumping.StaticInfo
{
    [CreateAssetMenu(fileName = "SkiJumpInfo", menuName = "Data/SkiJumpInfo")]
    public class SkiJumpInfoGlobal : ScriptableObject
    {
        [Header("Size")]
        public float K;
        public float HS;
        public float record;
        public string recordJumper;
        public float p;
        public float pointMeter;
        public float pointGate;
        public int gateDefault;

        [Header("wind")]
        public float gustsFreq; //0.001f
        public float gustsForce; // 0.01
        public float rotateForce;// 3
        public float windStartDirect;
        public float windStartForce;

        [Header("Hill")]


        [Header("InRun")]
        public float maxSpeed;
        public float increaseSpeed;
        public int startPosition;
        public int minPosition;
        public int maxPosition;
        public float maxPositionValue;
        public float onePoisitionDif;

        [Header("Up")]
        public float bestUp;
        public float lostHeightFactor;
        public float lostSpeedFactorLate;
        public float multiplyResistanceLate;
        public float areaUp;

        [Header("Fly")]
        public float resistance = 0.001f;
        public float controlValue = 0.4f;// 0,4
        public float divisionSwing = 2; // 2
        public float swingFrequencyFly = 0.1f; // 0.1
        public float swingFrequencyOther = 0.075f;// 0.075
        public float divisionWindSwingMIN = 20; // 20
        public float divisionWindSwingMAX = 15; // 15

        [Header("Down")]
        public float crashHeight; // 1.4  1.6   2   2.5
        public float badHeight;
        public float OkHeight;
        public float NiceHeight;
        public float resistanceHill = 1.5f;
        public float resistanceDown = 15;



        [Header("Other")]
        public string country3Dig;
        public string fullName;
        public float changeMeter;
        public float windFactor; //9

        private void Start()
        {

            //windStartDirect = 270;
            //windStartForce = 1;
            //gustsFreq = 0;
            //gustsForce = 0;
            //rotateForce = 0;
        }

        public Vector2 SetWind()
        {
            gustsFreq = Random.Range(0.001f, 0.1f);
            gustsForce = Random.Range(0.05f, 0.55f);
            rotateForce = Random.Range(0.1f, 2f);
            windStartDirect = Random.Range(0f, 359f);
            windStartForce = Random.Range(0.5f, 1.5f);

            return WindController.CalculateWind(this);
        }
    }
}