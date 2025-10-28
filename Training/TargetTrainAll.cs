using Assets.Scripts.Jumping.StaticInfo;
using Assets.Scripts.Jumping;
using Assets.Scripts.WebDTO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Assets.Scripts.Jumping.Controllers;

namespace Assets.Scripts.Training
{
    public class TargetTrainAll : ITrainingController
    {
        public float targetFlyX, targetFlyY,targetInRun, targetDown, tolerance;
        GameObject jumperObject;
        public float avg = 0;
        public float frames = 0;
        public float minDistance = 0;
        private float bestAvg = 1000;

        private FlyAnim fly;
        private LandingAnim landing;
        private InRunAnim inRun;


        private void Update()
        {
            if (!sceneObjects)
                return;
            if (jumper.jumpState != JumpState.Idle)
            {
                sceneObjects.ExtraInfoTraining.text = "AVG DIST: " + avg.ToString("0.00");
                sceneObjects.setActiveExtraInfo(true);
            }

            if (jumper.jumpState == JumpState.InRun)
            {
                if (!sceneObjects.HorizontalTarget.activeInHierarchy)
                    sceneObjects.HorizontalTarget.SetActive(true);

                avg = (avg * frames + Mathf.Abs(inRun.getAxis().Item1 - targetInRun)) / ++frames;
            }
            else if (jumper.jumpState == JumpState.Fly)
            {
                if (!sceneObjects.VerticalTarget.activeInHierarchy)
                {
                    sceneObjects.VerticalTarget.SetActive(true);
                    sceneObjects.HorizontalTarget.GetComponent<Slider>().value = targetFlyX;
                }

                avg = (avg * frames + Mathf.Abs(fly.getAxis().Item1 - targetFlyX)) / ++frames;
                avg = (avg * frames + Mathf.Abs(fly.getAxis().Item2 - targetFlyY)) / ++frames;
            }
            else if (jumper.jumpState == JumpState.Down && landing.enabled)
            {
                if (sceneObjects.VerticalTarget.activeInHierarchy)
                {
                    sceneObjects.VerticalTarget.SetActive(false);
                    sceneObjects.HorizontalTarget.GetComponent<Slider>().value = targetDown;
                }

                avg = (avg * frames + Mathf.Abs(landing.getAxis().Item1 - targetDown)) / ++frames;
            }
            else
            {
                sceneObjects.HorizontalTarget.SetActive(false);
                sceneObjects.VerticalTarget.SetActive(false);

                if (jumper.jumpState == JumpState.Down)
                    sceneObjects.setActiveExtraInfo(false);
            }
        }

        protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            base.OnSceneLoaded(scene, mode);
            if (scene.buildIndex == 0)
                return;
            jumperObject = sceneObjects.player;
            sceneObjects.HorizontalTarget.SetActive(true);
            sceneObjects.VerticalTarget.SetActive(true);
            sceneObjects.HorizontalTarget.GetComponent<Slider>().value = targetInRun;
            sceneObjects.VerticalTarget.GetComponent<Slider>().value = targetFlyY;
            if (jumperObject == null)
            {
                Debug.LogError("No found jumper");
            }
            else
            {
                fly = jumperObject.GetComponentInChildren<FlyAnim>();
                inRun = jumperObject.GetComponentInChildren<InRunAnim>();
                landing = jumperObject.GetComponentInChildren<LandingAnim>();
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
            targetFlyX = UnityEngine.Random.Range(-3f, 3f);
            tolerance = UnityEngine.Random.Range(0.75f + (0.35f * (3 - lvl)), 1.35f + (0.35f * (3 - lvl)));

            targetFlyY = UnityEngine.Random.Range(-3f, 3f);

            minDistance = UnityEngine.Random.Range(1.1f - (0.25f * (3 - lvl)), 1.2f - (0.25f * (3 - lvl))) * skiJumpInfo.p;

            targetDown = UnityEngine.Random.Range(-3f, 3f);
            targetInRun = UnityEngine.Random.Range(-3f, 3f);

            description = $"Your's task is holding fly positions slider as near as possible, X axis to {targetFlyX.ToString("0.0")} value, Y axis to {targetFlyY.ToString("0.0")} value, inrun near to {targetInRun.ToString("0.00")} and after landing {targetDown.ToString("0.00")}, with avarage difference less or equal to {tolerance.ToString("0.00")}. Yours result distance have to be > {minDistance.ToString("0.00")}. Hill: {skiJumpInfo.name}";
        }

        public override void SetResult(PointsStat stats, bool isShowResult = true)
        {
            if (avg < tolerance && stats.dist > minDistance && stats.sum > 0 && landing.getAxis().Item2 > 5)
            {
                result = 1;
                isEnd = true;
            }
            else if (avg < tolerance * 1.2f && stats.dist > minDistance && stats.sum > 0 && landing.getAxis().Item2 > 5)
            {
                result = 0.4f;
            }
            else if (avg < tolerance * 1.4f && stats.dist > minDistance && stats.sum > 0 && landing.getAxis().Item2 > 5)
            {
                result = 0.2f;
            }
            else
            {
                result = 0;
            }
            bestAvg = Mathf.Min(bestAvg, avg);
            avg = 0;
            frames = 0;
            attemptEnded++;
            sceneObjects.jumpResult.ShowStat(jumper, stats, "Factor: " + result, attemptEnded);
            StartCoroutine(StartNext());
        }

        public override void ReceiveExperience()
        {
            ShowInfo($"You receive {result.ToString("0.0")} factor in {attemptEnded} attempt(s). Best result is {bestAvg.ToString("0.0")}, and your task was be lower than {tolerance.ToString("0.0")}. So you increase yours stats by:" +
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