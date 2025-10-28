using Assets.Scripts.Jumping.StaticInfo;
using Assets.Scripts;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.WebDTO;

namespace Assets.Scripts.Training
{
    public class ToPointTraining : ITrainingController
    {
        public float distance, tolerance, minJudge;
        private float bestDist;

        public override (float, bool) DiffToBeat()
        {
            return (distance, true);
        }

        public override void SetResult(PointsStat stats, bool isShowResult = true)
        {
            float juryPoints = stats.pointsJury.Sum() - stats.pointsJury.Min() - stats.pointsJury.Max();
            if (juryPoints >= minJudge && Mathf.Abs(stats.dist - distance) <= tolerance)
            {
                result = Mathf.Max(result, 1f);
                isEnd = true;
            }
            else if (Mathf.Abs(stats.dist - distance) <= tolerance*1.2f)
            {
                result = Mathf.Max(result, 0.3f);
            }
            else if (juryPoints >= minJudge)
            {
                result = Mathf.Max(result, 0.1f);
            }
            else
            {
                result = Mathf.Max(result, 0);
            }

            bestDist = (Mathf.Abs(stats.dist - distance) < Mathf.Abs(bestDist - distance)) ? stats.dist : bestDist;
            attemptEnded++;
            sceneObjects.jumpResult.ShowStat(jumper, stats, "Factor: " + result, attemptEnded);
            StartCoroutine(StartNext());
        }

        public override void Refresh()
        {
            int lvl = jumper.getLevel();
            RandomHill();
            distance = UnityEngine.Random.Range(skiJumpInfo.p * (0.9f - 0.1f * (3 - lvl)), skiJumpInfo.K * (1.2f - 0.1f * (3 - lvl)));
            tolerance = UnityEngine.Random.Range(1 * (1f + 0.5f * (3 - lvl)), 4 * (1f + 0.3f * (3 - lvl)));
            minJudge = UnityEngine.Random.Range(35 - (skiJumpInfo.K - distance) / 3, 48 - (skiJumpInfo.K - distance) / 3);
            description = $"Your's task is jump to the selected distance {distance.ToString("0.0")} with +-{tolerance.ToString("0.00")} tolerance and receive good judge notes >={minJudge.ToString("0.0")}. Hill: {skiJumpInfo.name}";
        }

        public override void ReceiveExperience()
        {
            ShowInfo($"You receive {result.ToString("0.0")} factor in {attemptEnded} attempt(s). Best result is {bestDist.ToString("0.0")} , and your task was be between {(distance - tolerance).ToString("0.0")}-{(distance + tolerance).ToString("0.0")}. So you increase yours stats by:" +
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