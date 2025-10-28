using Assets.Scripts.Competition;
using Assets.Scripts.Competition.Other;
using Assets.Scripts.ImportData;
using Assets.Scripts.WebDTO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Competition.Controllers
{

    [RequireComponent(typeof(SkipRound))]
    public class HighFiveCompCtrl : ICompetitionControler
    {
        private List<CompetitorResult> resultCopy;
        int fightGroupSize = 5;
        int groupsNumber = -1;

        public override void SetStartList(List<Competitor> list, int difficulty)
        {
            this.groupSize = competitionSettings.groupSize;
            this.currentGroup = 1;
            this.difficulty = difficulty;
            competitors = list;
            //startList = list;
            resultsDetails = new List<CompetitorResult>();
            fastReferenceResult = new Dictionary<Competitor, CompetitorResult>();
            for (int i = 0; i < competitors.Count(); i++)
            {
                fastReferenceResult.Add(competitors[i], new CompetitorResult(i, competitors[i], i + 1, i + 1));
                resultsDetails.Add(fastReferenceResult[competitors[i]]);
            }
            leader = competitors[competitors.Count() - 1];
        }

        public override void SetResult(PointsStat stats, bool isShowResult = true)
        {
            base.SetResult(stats, isShowResult);
            CompetitorResult result = fastReferenceResult[competitors[curPlayer - 1]];

            if (result.rank < resultsDetails.Count() && result.points < resultsDetails[result.rank].points)
            {
                int tmp = roundNumber - 1 >= 0 ? competitionSettings.roundSetting[roundNumber - 1].competitorsInRound : resultsDetails.Count();
                if (roundNumber == 1)
                    tmp = fightGroupSize;

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
            if (curPlayer == competitors.Count() && roundNumber <= 1)
            {
                int i = 1;
                foreach (var res in resultsDetails)
                {
                    if (roundNumber == 1)
                    {
                        if (i <= 2)
                        {
                            res.isWinInPair = true;
                            res.isQualified = true;
                        }
                        else
                        {
                            res.isWinInPair = false;
                            res.isQualified = false;
                        }
                    }
                    else
                    {
                        if (i <= competitionSettings.roundSetting[roundNumber].competitorsInRound)
                        {
                            res.isQualified = true;
                        }
                        else
                        {
                            res.isWinInPair = false;
                            res.isQualified = false;
                        }
                    }
                    i++;
                }
            }
            if (isShowResult)
            {
                sceneObjects.jumpResult.ShowStat(stats, result);
                coroutineNextJump = StartCoroutine(StartNext());
            }
        }

        public override void CreateStartList()
        {
            int competitorsNumber = Mathf.Min(competitionSettings.roundSetting[roundNumber - 1].competitorsInRound - 1, competitors.Count() - 1);
            if (roundNumber == 1)
            {
                if (groupsNumber >= 0)
                {
                    groupsNumber--;
                    if (groupsNumber == 1)
                        groupSize = 1;
                }
                else
                {
                    int tmp = competitorsNumber + 1;
                    tmp -= tmp % fightGroupSize;
                    groupsNumber = tmp / fightGroupSize;
                    groupSize = groupsNumber;
                    currentGroup = 1;
                    competitorsNumber = fightGroupSize;
                    resultCopy = resultsDetails.ToList();
                }
                resultsDetails.Clear();
                int i = groupsNumber * fightGroupSize - 1;
                competitors.Clear();
                while (i >= (groupsNumber - 1) * fightGroupSize)
                {
                    competitors.Add(resultCopy.ElementAt(i).competitorObj);
                    resultCopy.ElementAt(i).lp = i + 1;
                    resultCopy.ElementAt(i).rank = ((groupsNumber) * fightGroupSize - i);
                    resultsDetails.Add(resultCopy.ElementAt(i));
                    i--;
                }
                competitors.Reverse();

            }
            else
            {
                groupSize = 1;
                resultsDetails = resultCopy;
                resultsDetails.Sort((CompetitorResult a, CompetitorResult b) => { return (int)Mathf.Sign(b.points - a.points); });
                competitors.Clear();
                int i = 0;
                int j = 1;
                CompetitorResult tmp;
                while (i < resultsDetails.Count())
                {
                    tmp = resultsDetails.ElementAt(i);
                    if (tmp.isWinInPair)
                    {
                        competitors.Add(tmp.competitorObj);
                        tmp.lp = j++;
                        tmp.rank = i + 1;
                    }
                    tmp.points = 0;
                    tmp.distances.Clear();
                    i++;
                }
                int luckyNumber = competitionSettings.roundSetting[roundNumber - 1].competitorsInRound - competitors.Count();
                i = 0;
                while (i < resultsDetails.Count() && luckyNumber > 0)
                {
                    tmp = resultsDetails.ElementAt(i);
                    if (!tmp.isWinInPair)
                    {
                        competitors.Add(tmp.competitorObj);
                        tmp.lp = j++;
                        tmp.rank = i + 1;
                        tmp.points = 0;
                        tmp.distances.Clear();
                        luckyNumber--;
                    }
                    i++;
                }
            }
            competitors.Reverse();
            curPlayer = 0;
        }
    }

}