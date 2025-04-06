using UnityEngine;

public class JumperInfo : MonoBehaviour
{
    [Header("InRun")]
    public float ControlFactorRun = 1;
    public float SwingsFactorRun = 1;
    public float lubricationFactor = 1;

    [Header("Fly")]
    public float ControlFactorFly = 1;
    public float SwingsFactorFly = 1;
    public float suitFactor = 1;

    [Header("Down")]
    public float ControlFactorDown = 1;
    public float SwingsFactorDown = 1;
    public float bootsFactor = 1;


    [Header("Settings")]
    public float MouseXSens = 1;
    public float MouseYSens = 1;
}
