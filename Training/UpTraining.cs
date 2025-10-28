using Assets.Scripts.WebDTO;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Jumping.StaticInfo;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Training
{
    public class UpTraining : ITrainingController
    {
        public float upDistance, tolerance;
        CinemachineSplineCart jumperCart;
        private float bestUp;

        protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            base.OnSceneLoaded(scene, mode);
            if (scene.buildIndex == 0)
                return;
            jumperCart = sceneObjects.player.GetComponent<CinemachineSplineCart>();
            if (jumperCart == null)
            {
                Debug.LogError("No found CinemachineSplineCart");
            }
        }

        private void Update()
        {
            if (sceneObjects == null)
                return;
            if (jumper.jumpState != JumpState.Down)
            {
                sceneObjects.setActiveExtraInfo(true);
                sceneObjects.ExtraInfoTraining.text = "POSITION: " + jumperCart.SplinePosition.ToString("0.00");
                bestUp = (Mathf.Abs(jumperCart.SplinePosition-upDistance)< Mathf.Abs(bestUp - upDistance))?  jumperCart.SplinePosition: bestUp;
            }
            else
            {
                sceneObjects.setActiveExtraInfo(false);
            }
        }

        public override (float, bool) DiffToBeat()
        {
            return (0, false);
        }

        public override void Refresh()
        {
            int lvl = jumper.getLevel();
            RandomHill();
            upDistance = UnityEngine.Random.Range(skiJumpInfo.bestUp - 4, skiJumpInfo.bestUp + 3);
            tolerance = UnityEngine.Random.Range(0.3f * (1f + 1f * (3 - lvl)), 1f * (1f + 1f * (3 - lvl)));
            description = $"Your's task is jump in inrun distance {upDistance.ToString("0.0")} with +-{tolerance.ToString("0.00")} tolerance. Hill: {skiJumpInfo.name}";
        }

        public override void SetResult(PointsStat stats, bool isShowResult = true)
        {
            if (Mathf.Abs(jumperCart.SplinePosition - upDistance) < tolerance)
            {
                result = 1;
                isEnd = true;
            }
            else if (Mathf.Abs(jumperCart.SplinePosition - upDistance) < tolerance * 1.4f)
            {
                result = 0.2f;
            }
            else
            {
                result = 0;
            }

            attemptEnded++;
            sceneObjects.jumpResult.ShowStat(jumper, stats, "Factor: " + result, attemptEnded);
            StartCoroutine(StartNext());
        }

        public override void ReceiveExperience()
        {
            ShowInfo($"You receive {result.ToString("0.0")} factor in {attemptEnded} attempt(s). Best result is {bestUp.ToString("0.0")}, and your task was be between {(upDistance-tolerance).ToString("0.0")}-{(upDistance+tolerance).ToString("0.0")}. So you increase yours stats by:" +
                $"\n Control Inrun: {trainingExperience.ctrlRUN*result}" +
                $"\n Control Fly: {trainingExperience.ctrlFLY * result}" +
                $"\n Control Landing: {trainingExperience.ctrlDown * result}" +
                $"\n Swings Inrun: {trainingExperience.swingRUN * result}" +
                $"\n Swings Fly: {trainingExperience.swingFLY * result}" +
                $"\n Swings Landing: {trainingExperience.swingDown * result}");
            base.ReceiveExperience();
        }
    }
}