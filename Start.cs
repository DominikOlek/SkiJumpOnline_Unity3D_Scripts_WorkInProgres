using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class StarLevel : MonoBehaviour
{
    public GameObject jumper,hill,controller;
    private Fly fly;
    private Points points;
    private SkiJumpInfo jumpInfo;
    private Vector3 startPos;
    private Quaternion startRot;
    [SerializeField] Slider sliderX, sliderY;
    private UiController uiController;

    private void Start()
    {

        uiController = GameObject.Find("Canvas").GetComponent<UiController>();
        QualitySettings.vSyncCount = 0; // Wyłączenie V-Sync
        Application.targetFrameRate = 60;
        Cursor.lockState = CursorLockMode.Confined;
        startPos = jumper.transform.position;
        startRot = jumper.transform.rotation;

        fly = jumper.GetComponent<Fly>();
        points=controller.GetComponent<Points>();
        jumpInfo = hill.GetComponent<SkiJumpInfo>();
        fly.enabled = false;
        fly.startAnim();
        sliderX.value = 0;
        sliderY.value = 0;
        //SetStart();
    }
    public void SetStart()
    {
        fly.enabled = false;
        points.enabled=false;
        jumper.transform.position= startPos;
        jumper.transform.rotation= startRot;
        fly.enabled = true;
        sliderX.value = 0;
        sliderY.value = 0;
        uiController.hideStat();
    }

    public void Restart()
    {
        SetStart();
        fly.enabled = false;
    }
}
