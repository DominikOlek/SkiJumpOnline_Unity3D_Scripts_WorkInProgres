using UnityEngine;

public class SkiJumpInfo : MonoBehaviour
{
    [Header("Size")]
    public float K;
    public float HS;
    public float p;
    public float pointMeter;

    [Header("Wind")]
    public float gustsFreq; //0.001f
    public float gustsForce; // 0.01
    public float rotateForce;// 3
    public float windStartDirect;
    public float windStartForce;

    [Header("Hill")]
    public float increaseSpeed;
    public float startPosition;
    public float startPositionValue;
    public float onePoisitionDif;
    public float bestUp;
    public float areaUp;
    public float controlValue = 0.4f;// 0,4
    public float divisionSwing =2; // 2
    public float swingFrequencyFly = 0.1f; // 0.1
    public float swingFrequencyOther = 0.075f;// 0.075
    public float divisionWindSwingMIN = 20; // 20
    public float divisionWindSwingMAX = 15; // 15

    [Header("Down")]
    public float crashHeight, badHeight, OkHeight, NiceHeight; // 1.4  1.6   2   2.5



    [Header("Other")]
    public float changeMeter;
    public float windFactor; //9
}
