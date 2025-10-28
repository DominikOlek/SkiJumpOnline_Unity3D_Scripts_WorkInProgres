using Assets.Scripts.ImportData;
using Assets.Scripts.Jumping.StaticInfo;
using Assets.Scripts.Jumping.UI;
using Assets.Scripts.Jumping;
using Assets.Scripts.SuitShop;
using Unity.Cinemachine;
using UnityEngine;
using Assets.Scripts.Competition;
using TMPro;
using Assets.Scripts.Jumping.Controllers;

namespace Assets.Scripts.Competition.Other
{
    public class SceneObjects : MonoBehaviour
    {
        public GameObject POV;
        public GameObject player, aIPlayer, controller, uI, skiJump;
        public JumperInfoGlobal aIJumperInfo, playerJumperInfo;
        public JumperOverlay jumperOverlay;
        public TextMeshProUGUI ExtraInfoTraining;
        public GameObject HorizontalTarget, VerticalTarget;

        [HideInInspector] public CinemachineCamera cineCamera;
        [HideInInspector] public CameraOptions cameraOptions;
        [HideInInspector] public Points pointsScript;
        [HideInInspector] public UiJumpResult jumpResult;
        [HideInInspector] public UIResultsController resultsController;
        [HideInInspector] public WindController windController;
        [HideInInspector] public SkiJumpInfo skiJumpInfoHolder;
        [HideInInspector] public SkiJumpInfoGlobal skijumpInfo;
        [HideInInspector] public Suits suits;
        [HideInInspector] public ToBeatLine toBeat;

        [HideInInspector] public Fly flyPlayer, flyAI;

        private void Awake()
        {
            flyPlayer = player.GetComponent<Fly>();
            flyAI = aIPlayer.GetComponent<Fly>();

            pointsScript = controller.GetComponent<Points>();
            windController = controller.GetComponent<WindController>();
            suits = controller.GetComponent<Suits>();

            skiJumpInfoHolder = skiJump.GetComponent<SkiJumpInfo>();
            skijumpInfo = skiJumpInfoHolder.skiJumpInfo;

            jumpResult = uI.GetComponent<UiJumpResult>();
            resultsController = uI.GetComponent<UIResultsController>();

            toBeat = GameObject.FindWithTag("ToBeat").GetComponent<ToBeatLine>();

            cameraOptions = POV.GetComponent<CameraOptions>();
            cineCamera = POV.GetComponent<CinemachineCamera>();
        }

        public void setActiveExtraInfo(bool status)
        {
            if (ExtraInfoTraining.transform.parent.gameObject.activeInHierarchy != status)
                ExtraInfoTraining.transform.parent.gameObject.SetActive(status);
        }
    }
}