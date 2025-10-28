using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using Assets.Scripts.Jumping;
using Assets.Scripts.Jumping.StaticInfo;
using Assets.Scripts.Jumping.UI;
using Assets.Scripts.Jumping.Controllers;

namespace Assets.Scripts.etc
{
    public class StarLevel : MonoBehaviour
    {
        public GameObject jumper, hill, controller, AIJumper;
        private Fly fly, fly2;
        private Points points;
        private SkiJumpInfo jumpInfo;
        private Vector3 startPos;
        private Quaternion startRot;
        [SerializeField] Slider sliderX, sliderY;
        private UiJumpResult uiJumpController;
        private UIResultsController uiResultsController;

        private void Start()
        {

            uiJumpController = GameObject.Find("SkiOverlay").GetComponent<UiJumpResult>();
            uiResultsController = GameObject.Find("SkiOverlay").GetComponent<UIResultsController>();
            QualitySettings.vSyncCount = 0; // Wyłączenie V-Sync
            Application.targetFrameRate = 60;
            Cursor.lockState = CursorLockMode.Confined;
            startPos = jumper.transform.position;
            startRot = jumper.transform.rotation;

            fly = jumper.GetComponent<Fly>();
            fly2 = AIJumper.GetComponent<Fly>();
            points = controller.GetComponent<Points>();
            jumpInfo = hill.GetComponent<SkiJumpInfo>();
            fly.enabled = false;
            fly.startAnim();
            sliderX.value = 0;
            sliderY.value = 0;
            SetStart();
        }
        public void SetStart()
        {
            fly.enabled = false;
            fly2.enabled = false;
            points.enabled = false;
            jumper.transform.position = startPos;
            jumper.transform.rotation = startRot;
            AIJumper.transform.position = startPos;
            AIJumper.transform.rotation = startRot;
            fly.enabled = true;
            fly2.enabled = true;
            sliderX.value = 0;
            sliderY.value = 0;
            uiJumpController.HideStat();
            uiResultsController.HideResults();
        }

        public void Restart()
        {
            SetStart();
            fly.enabled = false;
            //fly2.enabled = false;
        }
    }
}