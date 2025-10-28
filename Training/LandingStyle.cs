using Assets.Scripts.Animation;
using Assets.Scripts.Jumping;
using Assets.Scripts.Jumping.Controllers;
using Assets.Scripts.WebDTO;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Training
{
    public class LandingStyle : ITrainingController
    {
        public const int jumpsInAttempt = 3;

        public float distance;
        public Landing landingMethod; //0 - telemark, 1 - two legs
        private LandingAnim landing;
        public int jumpNumber = 0;
        public int bestLanding = 0;

        private void Update()
        {
            if(jumper.jumpState == Jumping.StaticInfo.JumpState.Down)
                sceneObjects.setActiveExtraInfo(false);
        }

        protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            base.OnSceneLoaded(scene, mode);
            if (scene.buildIndex == 0)
                return;
            if (sceneObjects.player == null)
            {
                Debug.LogError("No found jumper");
            }
            else
            {
                landing = sceneObjects.player.GetComponentInChildren<LandingAnim>();
            }
        }

        public override (float, bool) DiffToBeat()
        {
            return (distance, true);
        }

        public override void Refresh()
        {
            int lvl = jumper.getLevel();
            RandomHill();
            distance = UnityEngine.Random.Range(skiJumpInfo.p * (1.1f - 0.2f * (3 - lvl)), skiJumpInfo.K * (1.1f - 0.2f * (3 - lvl)));
            landingMethod = UnityEngine.Random.Range(0,2) == 0 ? Landing.TwoLegs: Landing.Telemark;
            description = $"Your's task is jump to the selected distance or longer with chosen landing style and no crash. You have to do it {jumpsInAttempt} times correct in line, and have {attempts} attempts. Hill: {skiJumpInfo.name}";
        }

        protected override void setData()
        {
            base.setData();
            sceneObjects.setActiveExtraInfo(true);
            sceneObjects.ExtraInfoTraining.text = "Landing style: " + landingMethod.ToString();
        }

        public override void SetResult(PointsStat stats, bool isShowResult = true)
        {
            if (jumper.landingStyle == landingMethod && stats.dist > distance && landing.getAxis().Item2 > 5)
            {
                jumpNumber++;
                bestLanding = Mathf.Max(bestLanding,jumpNumber);
                if (jumpNumber == jumpsInAttempt)
                {
                    isEnd = true;
                    result = Mathf.Max(result, 1f);
                }
            }
            else
            {
                jumpNumber = 0;
                attemptEnded++;
                result = Mathf.Max(result,0);
            }
            if (jumpNumber == 1)
                result = Mathf.Max(result, 0.1f);
            else if (jumpNumber == 2)
                result = Mathf.Max(result, 0.2f);
            Refresh();
            sceneObjects.jumpResult.ShowStat(jumper, stats, jumpNumber+"/"+jumpsInAttempt + " Factor: " + result, attemptEnded);
            StartCoroutine(StartNext());
        }

        public override void ReceiveExperience()
        {
            ShowInfo($"You receive {result.ToString("0.0")} factor in {attemptEnded} attempt(s). Best result is {bestLanding} times landing with correct style in line. So you increase yours stats by:" +
            $"\n Control Inrun: {trainingExperience.ctrlRUN * result}" +
            $"\n Control Fly: {trainingExperience.ctrlFLY * result}" +
                $"\n Control Landing: {trainingExperience.ctrlDown * result}" +
                $"\n Swings Inrun: {trainingExperience.swingRUN * result}" +
                $"\n Swings Fly: {trainingExperience.swingFLY * result}" +
                $"\n Swings Landing: {trainingExperience.swingDown * result}");
            base.ReceiveExperience();
        }
    }
}