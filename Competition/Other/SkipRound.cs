using Assets.Scripts.Competition.Controllers;
using Assets.Scripts.ImportData;
using Assets.Scripts.Jumping;
using Assets.Scripts.Jumping.Controllers;
using Assets.Scripts.Jumping.StaticInfo;
using Assets.Scripts.WebDTO;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Competition.Other
{
    public class SkipRound : MonoBehaviour
    {
        SceneObjects sceneObjects;

        WindController windController;
        ICompetitionControler competitionControler;
        SkiJumpInfo skiJumpInfoHolder;
        SkiJumpInfoGlobal skiJumpInfo;

        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex == 0)
                return;
            GameObject tmp = GameObject.FindGameObjectsWithTag("SceneData").FirstOrDefault();
            if (tmp == null)
            {
                Debug.LogError("Not Found SceneData");
                return;
            }
            sceneObjects = tmp.GetComponent<SceneObjects>();
            windController = sceneObjects.windController;
            skiJumpInfo = sceneObjects.skijumpInfo;
        }

        public void DestroyOnLoad()
        {
            SceneManager.MoveGameObjectToScene(GameObject.Find("Controllers"), SceneManager.GetActiveScene());
        }

        private void Start()
        {
            competitionControler = GetComponent<ICompetitionControler>();
        }

        public void SkipJump(AIInfo[] jumpers)
        {
            sceneObjects.setActiveExtraInfo(false);
            int i = 0;
            foreach (AIInfo jumper in jumpers)
            {
                float result = -2.758f * windController.getDir() - 7.299f * windController.getSpeed() + 349.063f * jumper.controlFactorRun
                    + 165.76f * jumper.controlFactorFly - 181.186f * jumper.controlFactorDown + 88.525f * jumper.swingsFactorRun +
                    130.369f * jumper.swingsFactorFly - 255.985f * jumper.swingsFactorDown + 283.296f * jumper.lubricationFactor +
                    172.554f * jumper.suitFactor + 102.497f * jumper.bootsFactor - 770.806f + ((skiJumpInfo.startPosition - skiJumpInfo.gateDefault) * skiJumpInfo.pointGate);

                result *= (1 + 0.02f * (skiJumpInfo.startPosition - 4));

                float sum = result / 2 + Random.Range(-Mathf.Abs(result / 40), Mathf.Abs(result / 40));
                float dist = skiJumpInfo.K + ((sum - 60) / skiJumpInfo.pointMeter);
                float judge = 0;
                switch (competitionControler.getDifLevel())
                {
                    case 3:
                        judge = Random.Range(Mathf.Max(48 + (dist - skiJumpInfo.HS) / 2, 0), Mathf.Min(56 + (dist - skiJumpInfo.HS) / 2, 60));
                        break;
                    case 2:
                        judge = Random.Range(Mathf.Max(42 + (dist - skiJumpInfo.HS) / 2, 0), Mathf.Min(50 + (dist - skiJumpInfo.HS) / 2, 60));
                        break;
                    case 1:
                        judge = Random.Range(Mathf.Max(36 + (dist - skiJumpInfo.HS) / 2, 0), Mathf.Min(44 + (dist - skiJumpInfo.HS) / 2, 60));
                        break;
                }
                dist = skiJumpInfo.K + ((sum - judge - 60) / skiJumpInfo.pointMeter);
                PointsStat stat = new PointsStat()
                {
                    sum = sum,
                    dist = dist,
                    windY = windController.getSpeed(),
                };

                competitionControler.SetResult(stat, false);
                if (i + 1 < jumpers.Length && jumpers[i + 1] == null)
                {
                    competitionControler.RunNextCompetitor();
                    break;
                }
                else if (i == jumpers.Length - 1)
                {
                    competitionControler.RunNextCompetitor();
                }
                i++;
            }

        }
    }
}