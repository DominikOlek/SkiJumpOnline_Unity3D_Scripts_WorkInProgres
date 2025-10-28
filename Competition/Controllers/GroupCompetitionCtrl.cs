using Assets.Scripts.Competition;
using Assets.Scripts.Competition.Other;
using Assets.Scripts.ImportData;
using Assets.Scripts.WebDTO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Competition.Controllers
{

    [RequireComponent(typeof(SkipRound))]
    public class GroupCompetitionCtrl : ICompetitionControler
    {
        Competitor curentPersonCompetitor;

        public override void SetStartList(List<Competitor> list, int difficulty)
        {
            this.groupSize = competitionSettings.groupSize;
            this.currentGroup = 1;
            this.difficulty = difficulty;
            competitors = list;
            resultsDetails = new List<CompetitorResult>();
            fastReferenceResult = new Dictionary<Competitor, CompetitorResult>();
            for (int i = 0; i < competitors.Count(); i++)
            {
                fastReferenceResult.Add(competitors[i], new CompetitorResult(i, competitors[i], i + 1, i + 1));
                resultsDetails.Add(fastReferenceResult[competitors[i]]);
            }
            leader = competitors[competitors.Count() - 1];
        }

        public override void SetRecord(PointsStat stats, CompetitorResult result)
        {
            if (stats.dist > skiJumpInfo.record)
            {
                skiJumpInfo.record = stats.dist;
                skiJumpInfo.recordJumper = curentPersonCompetitor.getName();
            }
        }

        public override void SetData(Competitor competitor)
        {
            curentPersonCompetitor = competitor.GetNextCompetitor();
            plastronNumberCtrl.ChangeNumber(fastReferenceResult[competitors[curPlayer]].lp * 10 + currentGroup, competitor == leader);
            if (!curentPersonCompetitor.isAI()) // player
            {
                EnablePlayer();
            }
            else // AI
            {
                EnableAI(curentPersonCompetitor);
            }
        }

        public override void SetResult(PointsStat stats, bool isShowResult = true)
        {
            base.SetResult(stats, isShowResult);
            CompetitorResult result = fastReferenceResult[competitors[curPlayer - 1]];

            if (result.rank < resultsDetails.Count() && result.points < resultsDetails[result.rank].points)
            {
                int tmp = roundNumber - 1 >= 0 ? competitionSettings.roundSetting[roundNumber - 1].competitorsInRound : resultsDetails.Count();
                for (int i = result.rank; i < tmp; i++)
                {
                    if (resultsDetails[i].points <= resultsDetails[i - 1].points)
                    {
                        result.rank = i;
                        break;
                    }
                    else
                    {
                        resultsDetails[i].rank -= 1;
                        if (roundNumber < competitionSettings.roundSetting.Length && resultsDetails[i].rank <= competitionSettings.roundSetting[roundNumber].competitorsInRound)
                        {
                            resultsDetails[i].isQualified = true;
                        }
                        else
                        {
                            resultsDetails[i].isQualified = false;
                        }
                        result.rank = i - 1;
                        (resultsDetails[i - 1], resultsDetails[i]) = (resultsDetails[i], resultsDetails[i - 1]);
                    }
                }
            }
            else
            {
                for (int i = result.rank - 2; i >= 0; i--)
                {
                    if (resultsDetails[i].points >= resultsDetails[i + 1].points)
                    {
                        result.rank = i + 2;
                        break;
                    }
                    else
                    {
                        resultsDetails[i].rank += 1;
                        if (roundNumber < competitionSettings.roundSetting.Length && resultsDetails[i].rank <= competitionSettings.roundSetting[roundNumber].competitorsInRound)
                        {
                            resultsDetails[i].isQualified = true;
                        }
                        else
                        {
                            resultsDetails[i].isQualified = false;
                        }
                        result.rank = i + 1;
                        (resultsDetails[i + 1], resultsDetails[i]) = (resultsDetails[i], resultsDetails[i + 1]);
                    }
                }
            }
            if (roundNumber < competitionSettings.roundSetting.Length && result.rank <= competitionSettings.roundSetting[roundNumber].competitorsInRound)
            {
                result.isQualified = true;
            }
            else
            {
                result.isQualified = false;
            }
            if (isShowResult)
            {
                Competitor tmp = result.competitorObj;
                result.competitorObj = curentPersonCompetitor;
                sceneObjects.jumpResult.ShowStat(stats, result);
                result.competitorObj = tmp;
                coroutineNextJump = StartCoroutine(StartNext());
            }
        }

        public override void RunNextCompetitor()
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
                if (!curentPersonCompetitor.isAI())
                    sceneObjects.flyPlayer.enabled = false;
                else
                    sceneObjects.flyAI.enabled = false;
                SetData(competitors[curPlayer]);
                ShowJumperInfo(curPlayer);
            }
        }

        protected override void ShowJumperInfo(int posInStartList)
        {
            if (coroutineInfoHide != null)
                StopCoroutine(coroutineInfoHide);
            sceneObjects.jumperOverlay.Hide();
            sceneObjects.jumperOverlay.Show(fastReferenceResult[competitors[posInStartList]], currentGroup + " " + curentPersonCompetitor.getName());

            coroutineInfoHide = StartCoroutine(HideInfo());
        }

        public override void SkipAI(int number)
        {
            if (coroutineNextJump != null)
            {
                StopCoroutine(coroutineNextJump);
            }
            sceneObjects.flyAI.enabled = false;
            sceneObjects.flyAI.enabled = true;
            number = Mathf.Min(number, competitors.Count() - curPlayer);
            AIInfo[] list = new AIInfo[number];
            int i = 0;
            list[i++] = aIList.AIJumpers[curentPersonCompetitor.getAiId()];
            int id = curPlayer + 1;
            while (i < number && competitors[id].isAI())
            {
                list[i++] = aIList.AIJumpers[competitors[id++].GetNextCompetitor().getAiId()];
            }
            skipRound.SkipJump(list);
        }

        public override void CreateStartList()
        {
            int competitorsNumber = competitors.Count() - 1;
            if (roundNumber > 0)
                competitorsNumber = Mathf.Min(competitionSettings.roundSetting[roundNumber - 1].competitorsInRound - 1, competitors.Count() - 1);
            int i = competitorsNumber;
            competitors.Clear();
            while (i >= 0)
            {
                competitors.Add(resultsDetails.ElementAt(i).competitorObj);
                if (roundNumber > 1 || currentGroup > 1)
                {
                    resultsDetails.ElementAt(i).lastRoundRank = i + 1;
                }
                else
                {
                    resultsDetails.ElementAt(i).lp = competitorsNumber - i + 1;
                    resultsDetails.ElementAt(i).lastRoundRank = -1;
                }
                i--;
            }
            curPlayer = 0;
        }
    }
}