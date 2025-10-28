using Assets.Scripts.Competition.Other;
using Assets.Scripts.etc;
using Assets.Scripts.ImportData;
using Assets.Scripts.Jumping;
using Assets.Scripts.Jumping.Controllers;
using Assets.Scripts.Jumping.StaticInfo;
using Assets.Scripts.WebDTO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Competition.Controllers
{
    [Serializable]
    public abstract class ICompetitionControler : MonoBehaviour , IToBeat, ISetResult, IGetDifficulty
    {
        public bool isGroup = false;
        public string shortDescription;
        protected AIList aIList;
        protected SceneObjects sceneObjects;
        protected SkiJumpInfoGlobal skiJumpInfo;
        protected SkipRound skipRound;
        protected DataHolder dataHolder;
        protected PlastronNumberCtrl plastronNumberCtrl;

        protected List<Competitor> competitors;
        protected int curPlayer = 0;
        protected int roundNumber = 0;
        protected Dictionary<Competitor, CompetitorResult> fastReferenceResult;
        protected List<CompetitorResult> resultsDetails;
        public CompetitionSettings competitionSettings;
        protected AIInfo aIJumper;
        protected int difficulty;

        protected Coroutine coroutineNextJump, coroutineInfoHide;
        protected int groupSize;
        protected int currentGroup;
        protected Competitor leader;

        void Start()
        {
            aIList = GameObject.Find("ImportController").GetComponent<AIList>();
            if (aIList == null)
                Debug.LogError("Not found ImportController");
        }

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
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
            sceneObjects.resultsController.SetNextRoundButton("NEXT ROUND", () => { RunNextRound(); });
            skiJumpInfo = sceneObjects.skijumpInfo;

            skipRound = GetComponent<SkipRound>();
            roundNumber = competitionSettings.qualification ? 0 : 1;
            dataHolder = GameObject.Find("DataHolder").GetComponent<DataHolder>();

            tmp = GameObject.FindGameObjectWithTag("PlastronNumberController");
            if (tmp == null)
            {
                Debug.LogError("Not Found PlastronNumberController");
                return;
            }
            plastronNumberCtrl = tmp.GetComponent<PlastronNumberCtrl>();

            SetData(competitors.First());
            ShowJumperInfo(0);
        }

        public void DestroyOnLoad()
        {
            SceneManager.MoveGameObjectToScene(GameObject.Find("Controllers"), SceneManager.GetActiveScene());
        }

        public void SetSetting(CompetitionSettings competition)
        {
            competitionSettings = competition;
        }

        public abstract void SetStartList(List<Competitor> list, int difficulty);

        /// <summary>
        /// Change curPlayer to next
        /// </summary>
        /// <param name="stats"></param>
        /// <param name="isShowResult"></param>
        public virtual void SetResult(PointsStat stats, bool isShowResult = true)
        {
            CompetitorResult result = fastReferenceResult[competitors[curPlayer++]];
            if (result.points == -12345)
                result.points = 0;
            result.points += stats.sum;
            result.distances.Add(stats.dist);
            //result.windx += stats.windX;
            result.windy += stats.windY;
            SetRecord(stats,result);
        }

        public virtual void SetRecord(PointsStat stats, CompetitorResult result)
        {
            if (stats.dist > skiJumpInfo.record)
            {
                skiJumpInfo.record = stats.dist;
                skiJumpInfo.recordJumper = result.Name;
            }
        } 


        public Dictionary<Competitor, CompetitorResult> GetResult()
        {
            return fastReferenceResult;
        }

        public virtual void RunNextCompetitor()
        {
            sceneObjects.pointsScript.enabled = false;
            sceneObjects.jumpResult.HideStat();

            if (curPlayer >= competitors.Count())
            {
                sceneObjects.flyAI.enabled = false;
                sceneObjects.flyPlayer.enabled = false;
                ShowResult();
            }
            else
            {
                if (!competitors[curPlayer - 1].isAI())
                    sceneObjects.flyPlayer.enabled = false;
                else
                    sceneObjects.flyAI.enabled = false;
                SetData(competitors[curPlayer]);
                ShowJumperInfo(curPlayer);
            }
        }

        public virtual (float, bool) DiffToBeat()
        {
            if(curPlayer < 0 || curPlayer >= competitors.Count()|| resultsDetails == null || resultsDetails.Count()< 1)
            {
                return (0,false);
            }
            float curJumper = Mathf.Max(fastReferenceResult[competitors[curPlayer]].points,0);
            var best = resultsDetails.FirstOrDefault();
            if (best == null) {
                return (0, false);
            }
            return (best.points - curJumper, false);
        }

        protected virtual void ShowJumperInfo(int posInStartList)
        {
            if (coroutineInfoHide != null)
                StopCoroutine(coroutineInfoHide);
            sceneObjects.jumperOverlay.Hide();
            if (posInStartList != competitors.Count() - 1)
                sceneObjects.jumperOverlay.Show(fastReferenceResult[competitors[posInStartList]],"NEXT: "+ fastReferenceResult[competitors[posInStartList + 1]].Name);
            else
                sceneObjects.jumperOverlay.Show(fastReferenceResult[competitors[posInStartList]], "");

            coroutineInfoHide = StartCoroutine(HideInfo());
        }

        public virtual void SetData(Competitor competitor)
        {
            plastronNumberCtrl.ChangeNumber(fastReferenceResult[competitors[curPlayer]].lp,competitor == leader);
            if (!competitor.isAI()) // player
            {
                EnablePlayer();
            }
            else // AI
            {
                EnableAI(competitor);
            }
        }

        protected void EnablePlayer()
        {
            sceneObjects.suits.clothe(dataHolder.playerSuit);
            sceneObjects.player.SetActive(true);
            sceneObjects.aIPlayer.SetActive(false);
            sceneObjects.flyPlayer.enabled = true;
            sceneObjects.cineCamera.Follow = sceneObjects.player.transform;
            sceneObjects.cineCamera.LookAt = sceneObjects.player.transform;
            sceneObjects.cameraOptions.SetInfo(sceneObjects.playerJumperInfo);
            sceneObjects.pointsScript.jumper = sceneObjects.player;
            Cursor.lockState = CursorLockMode.Locked;
        }

        protected void EnableAI(Competitor competitor)
        {
            aIJumper = aIList.AIJumpers[competitor.getAiId()];
            sceneObjects.suits.clothe(aIJumper.curSuitID);
            sceneObjects.player.SetActive(false);
            sceneObjects.aIPlayer.SetActive(true);
            sceneObjects.flyAI.enabled = true;
            sceneObjects.aIJumperInfo.controlFactorDown = aIJumper.controlFactorDown;
            sceneObjects.aIJumperInfo.swingsFactorDown = aIJumper.swingsFactorDown;
            sceneObjects.aIJumperInfo.lubricationFactor = aIJumper.lubricationFactor;
            sceneObjects.aIJumperInfo.controlFactorFly = aIJumper.controlFactorFly;
            sceneObjects.aIJumperInfo.swingsFactorFly = aIJumper.swingsFactorFly;
            sceneObjects.aIJumperInfo.suitFactor = aIJumper.suitFactor;
            sceneObjects.aIJumperInfo.controlFactorRun = aIJumper.controlFactorRun;
            sceneObjects.aIJumperInfo.swingsFactorRun = aIJumper.swingsFactorRun;
            sceneObjects.aIJumperInfo.bootsFactor = aIJumper.bootsFactor;
            sceneObjects.aIJumperInfo.name = aIJumper.Name;
            sceneObjects.aIJumperInfo.Country3dig = aIJumper.Country;
            sceneObjects.cineCamera.Follow = sceneObjects.aIPlayer.transform;
            sceneObjects.cineCamera.LookAt = sceneObjects.aIPlayer.transform;
            sceneObjects.cameraOptions.SetInfo(sceneObjects.aIJumperInfo);
            sceneObjects.pointsScript.jumper = sceneObjects.aIPlayer;
            Cursor.lockState = CursorLockMode.Confined;
        }


        public void ShowResult()
        {
            for(int i =0;i< resultsDetails.Count(); i++)
            {
                resultsDetails.ElementAt(i).rank = i + 1;
            }
            sceneObjects.player.SetActive(false);
            sceneObjects.aIPlayer.SetActive(false);
            sceneObjects.jumperOverlay.Hide();
            sceneObjects.resultsController.ShowResults(resultsDetails,roundNumber);
            Cursor.lockState = CursorLockMode.Confined;
        }

        public void RunNextRound()
        {
            sceneObjects.resultsController.HideResults();
            currentGroup++;
            if (currentGroup > groupSize)
            { 
                roundNumber++;
                currentGroup = 1;
            }
            if (roundNumber == 1 && currentGroup == 1)
            {
                foreach (var el in resultsDetails)
                {
                    el.distances.Clear();
                    el.points = 0;
                }
            }
            if (roundNumber > 0)
            {
                if ((roundNumber == competitionSettings.roundSetting.Length && currentGroup == groupSize) || (roundNumber < competitionSettings.roundSetting.Length && competitionSettings.roundSetting[roundNumber].competitorsInRound == 0))
                {
                    sceneObjects.resultsController.SetNextRoundButton("Quit");
                }
                if (roundNumber == competitionSettings.roundSetting.Length + 1 || (roundNumber - 1 < competitionSettings.roundSetting.Length && competitionSettings.roundSetting[roundNumber - 1].competitorsInRound == 0))
                {
                    SceneManager.LoadScene(0);
                    return;
                }
            }
            CreateStartList();
            SetData(competitors.First());
            ShowJumperInfo(0);
        }

        public abstract void CreateStartList();

        public virtual void SkipAI(int number)
        {
            if (coroutineNextJump != null)
            {
                StopCoroutine(coroutineNextJump);
            }
            sceneObjects.flyAI.enabled = false;
            sceneObjects.flyAI.enabled = true;
            number = Mathf.Min(number, competitors.Count() - curPlayer);
            AIInfo[] list = new AIInfo[number];
            int id = curPlayer;
            int i = 0;
            while (i < number && competitors[id].isAI())
            {
                list[i++] = aIList.AIJumpers[competitors[id++].getAiId()];
            }
            skipRound.SkipJump(list);
        }

        protected IEnumerator StartNext()
        {
            yield return new WaitForSeconds(3);
            coroutineNextJump = null;
            RunNextCompetitor();
        }

        protected IEnumerator HideInfo()
        {
            yield return new WaitForSeconds(5);
            sceneObjects.jumperOverlay.Hide();
        }

        public int getDifLevel()
        {
            return difficulty;
        }
    }
    //    void RunNextCompetitor();
    //    void RunNextRound();
    //    void SetData(Competitor competitor);
    //    void SetResult(PointsStat stats, bool isShowResult = true);
    //    void SetStartList(List<Competitor> list);
    //    void ShowResult();
    //    void SkipAI(int number);

    //    void SetSetting(Competition competition);
    //    Dictionary<Competitor, CompetitorResult> GetResult();
    //}

    public class CompetitorResult : IComparable<CompetitorResult>
    {
        public int id;
        public List<float> distances = new List<float>();
        public float points = -12345;
        public int rank;
        public int lp;
        public bool isWinInPair = true;
        public int lastRoundRank = -1;
        public Competitor competitorObj = null;
        public bool isQualified = false;

        //public float windx = 0;
        public float windy = 0;

        public CompetitorResult(int id_, Competitor competitor, int rank, int lp)
        {
            id = id_;
            this.rank = rank;
            this.lp = lp;
            this.competitorObj = competitor;
        }

        public string Name => competitorObj.getName();


        public String Country()
        {
            return competitorObj.getCountry();
        }


        public int CompareTo(CompetitorResult other)
        {
            return Math.Sign(other.points - this.points);
        }
    }
}

