using Assets.Scripts.Competition;
using Assets.Scripts.Competition.Other;
using Assets.Scripts.ImportData;
using Assets.Scripts.Jumping.StaticInfo;
using Assets.Scripts.WebDTO;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Competition.Controllers
{
    [RequireComponent(typeof(SkipRound))]
    public class KOCompetitionCtrl : ICompetitionControler
    {
        private bool isKO = false;
        private bool isSecond = false;
        private PointsStat firstResult;
        private int luckyNumber;
        List<Competitor> luckyCompetitors;

        public override void SetStartList(List<Competitor> list,int difficulty)
        {
            this.groupSize = competitionSettings.groupSize;
            this.currentGroup = 1;
            this.difficulty = difficulty;
            isKO = competitionSettings.roundSetting[roundNumber].isKO;
            if (!isKO)
            {
                competitors = list;
                resultsDetails = new List<CompetitorResult>();
                fastReferenceResult = new Dictionary<Competitor, CompetitorResult>();
                for (int i = 0; i < competitors.Count(); i++)
                {
                    fastReferenceResult.Add(competitors[i], new CompetitorResult(i, competitors[i], i+1, i+1));
                    resultsDetails.Add(fastReferenceResult[competitors[i]]);
                }
            }
            else
            {
                SetPairList(list);
            }
            leader = competitors[competitors.Count() - 1];
        }

        private void SetPairList(List<Competitor> list)
        {
            competitors = new List<Competitor>();
            int i = Mathf.CeilToInt(list.Count() / 2f);
            resultsDetails = new List<CompetitorResult>();
            fastReferenceResult = new Dictionary<Competitor, CompetitorResult>();
            int j;
            for (j = i-1; i < list.Count(); i++)
            {
                competitors.Add(list[i]);
                fastReferenceResult.Add(competitors.Last(), new CompetitorResult(i, competitors.Last(), i+1, i+1));
                resultsDetails.Add(fastReferenceResult[competitors.Last()]);

                competitors.Add(list[j]);
                fastReferenceResult.Add(competitors.Last(), new CompetitorResult(j, competitors.Last(), j + 1, j + 1));
                resultsDetails.Add(fastReferenceResult[competitors.Last()]);
                j--;
            }
            if(j == 0)
            {
                competitors.Add(list[j]);
                fastReferenceResult.Add(competitors.Last(), new CompetitorResult(j, competitors.Last(), j+1,j+1));
                resultsDetails.Add(fastReferenceResult[competitors.Last()]);
            }

            resultsDetails.Sort((a,b)=> { return (int)Mathf.Sign(a.lp - b.lp); });

        }

        public override void SetResult(PointsStat stats, bool isShowResult = true)
        {
            base.SetResult(stats, isShowResult);
            CompetitorResult result = fastReferenceResult[competitors[curPlayer-1]];
            if (isKO)
            {
                if (isSecond)
                {
                    CompetitorResult result2 = fastReferenceResult[competitors[curPlayer-2]];
                    if (stats.sum > firstResult.sum)
                    {
                        result2.isWinInPair = false;
                        luckyCompetitors.Add(result2.competitorObj);
                        luckyCompetitors.Sort((a, b) => { return (int)Mathf.Sign(fastReferenceResult[b].points - fastReferenceResult[a].points); });
                        if (luckyCompetitors.Count() > luckyNumber) // it is ok becouse luckynumber is small
                        {
                            Debug.Log(luckyCompetitors.ElementAt(luckyNumber).getName());
                            fastReferenceResult[luckyCompetitors.ElementAt(luckyNumber)].isQualified = false;
                            luckyCompetitors.RemoveAt(luckyNumber);
                        }
                        if (luckyCompetitors.Contains(result2.competitorObj))
                            result2.isQualified = true;
                        result.isWinInPair = true;
                        result.isQualified = true;
                    }
                    else if (stats.sum < firstResult.sum)
                    {
                        result.isWinInPair = false;
                        luckyCompetitors.Add(result.competitorObj);
                        luckyCompetitors.Sort((a, b) => { return (int)Mathf.Sign(fastReferenceResult[b].points - fastReferenceResult[a].points); });
                        if (luckyCompetitors.Count() > luckyNumber)
                        {
                            Debug.Log(luckyCompetitors.ElementAt(luckyNumber).getName());
                            fastReferenceResult[luckyCompetitors.ElementAt(luckyNumber)].isQualified = false;
                            luckyCompetitors.RemoveAt(luckyNumber);
                        }
                        if (luckyCompetitors.Contains(result.competitorObj))
                            result.isQualified = true;
                        result2.isWinInPair = true;
                        result2.isQualified = true;
                    }
                    else {
                        result2.isWinInPair = true;
                        result.isWinInPair = true;
                        result2.isQualified = true;
                        result.isQualified = true;
                    }
                }
                else
                {
                    firstResult = stats;
                }
                isSecond = !isSecond;
            }

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
                        if (!isKO)
                        {
                            if (roundNumber < competitionSettings.roundSetting.Length && resultsDetails[i].rank <= competitionSettings.roundSetting[roundNumber].competitorsInRound)
                            {
                                resultsDetails[i].isQualified = true;
                            }
                            else
                            {
                                resultsDetails[i].isQualified = false;
                            }
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
                        if (!isKO)
                        {
                            if (roundNumber < competitionSettings.roundSetting.Length && resultsDetails[i].rank <= competitionSettings.roundSetting[roundNumber].competitorsInRound)
                            {
                                resultsDetails[i].isQualified = true;
                            }
                            else
                            {
                                resultsDetails[i].isQualified = false;
                            }
                        }
                        result.rank = i + 1;
                        (resultsDetails[i + 1], resultsDetails[i]) = (resultsDetails[i], resultsDetails[i + 1]);
                    }
                }
            }
            if (!isKO)
            {
                if (roundNumber < competitionSettings.roundSetting.Length && result.rank <= competitionSettings.roundSetting[roundNumber].competitorsInRound)
                {
                    result.isQualified = true;
                }
                else
                {
                    result.isQualified = false;
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
            int competitorsNumber2 = Mathf.Min(competitionSettings.roundSetting[roundNumber].competitorsInRound - 1, competitors.Count() - 1);
            int diff = competitorsNumber2 + 1 - (Mathf.CeilToInt(competitorsNumber / 2.0f));
            luckyNumber = Mathf.Max(0, diff);
            luckyCompetitors = new List<Competitor>();
            isKO = roundNumber < competitionSettings.roundSetting.Length ? competitionSettings.roundSetting[roundNumber].isKO : false;
            if (!isKO)
            {
                int j = competitorsNumber;
                int i = competitors.Count() - 1;
                competitors.Clear();
                while (i >= 0)
                {
                    var item = resultsDetails.ElementAt(i);
                    if (item.isQualified)
                    {
                        item.isWinInPair = true;
                        competitors.Add(item.competitorObj);
                        if (roundNumber > 1)
                            item.lastRoundRank = i + 1;
                        else
                            item.lp = i + 1;
                    }
                    i--;
                }
            }
            else
            {
                int i = Mathf.CeilToInt((competitorsNumber+1) / 2.0f);
                int toMove = resultsDetails.Count(a => !a.isWinInPair && a.rank < i && !luckyCompetitors.Contains(a.competitorObj));
                i += toMove;
                competitors.Clear();
                int j = i - 1;
                int howmany = competitorsNumber;
                while (howmany > 0)
                {
                    while (!resultsDetails.ElementAt(i).isQualified)
                    {
                        i++;
                    }
                    competitors.Add(resultsDetails.ElementAt(i).competitorObj);
                    if (roundNumber > 1)
                        resultsDetails.ElementAt(i).lastRoundRank = i + 1;
                    else
                        resultsDetails.ElementAt(i).lp = i + 1;
                    i++;
                    howmany--;

                    while (!resultsDetails.ElementAt(j).isQualified)
                    {
                        j--;
                    }
                    competitors.Add(resultsDetails.ElementAt(j).competitorObj);
                    if (roundNumber > 1)
                        resultsDetails.ElementAt(j).lastRoundRank = j + 1;
                    else
                        resultsDetails.ElementAt(j).lp = j + 1;
                    j--;
                    howmany--;
                }
                if (j == 0)
                {
                    if (resultsDetails.ElementAt(j).isQualified)
                    {
                        competitors.Add(resultsDetails.ElementAt(j).competitorObj);
                    }
                }
            }

            //if (luckyNumber > 0)
            //{
            //    foreach (var item in resultsDetails)
            //    {
            //        if (!item.isWinInPair)
            //        {
            //            luckyIndexs.Add(item.rank - 1);
            //            if (luckyIndexs.Count() == luckyNumber)
            //                break;
            //        }
            //    }
            //}
            //else
            //{
            //    for(int i = competitors.Count() -1; i>=0 && diff>0; i--)
            //    {
            //        if (resultsDetails[i].isWinInPair)
            //        {
            //            luckyIndexs.Add(i);
            //            diff--;
            //        }
            //    }
            //}

            //isKO = roundNumber < competitionSettings.roundSetting.Length ? competitionSettings.roundSetting[roundNumber].isKO: false;
            //if (isKO) {
            //    int i = Mathf.CeilToInt((competitorsNumber+1) / 2.0f);
            //    int toMove = resultsDetails.Count(a => !a.isWinInPair && a.rank < i && !luckyIndexs.Contains(a.rank - 1));
            //    i += toMove;
            //    competitors.Clear();
            //    int j = i - 1;
            //    int howmany = competitorsNumber;
            //    while(howmany > 0)
            //    {
            //        while (!AddPlayerToStartList(ref i, ref luckyIndexs, luckyNumber > 0))
            //        {
            //            i++;
            //        }
            //        howmany--;
            //        i += 2;

            //        while (!AddPlayerToStartList(ref j, ref luckyIndexs, luckyNumber > 0))
            //        {
            //            j--;
            //        }
            //        howmany--;
            //    }
            //    if (j == 0)
            //    {
            //        AddPlayerToStartList(ref j, ref luckyIndexs, luckyNumber > 0);
            //    }
            //}
            //else
            //{
            //    int i = competitorsNumber;
            //    int j = competitors.Count() - 1;
            //    competitors.Clear();
            //    while (i >= 0 && j>=0)
            //    {
            //        if (!AddPlayerToStartList(ref j, ref luckyIndexs, luckyNumber > 0))
            //            j--;
            //        else
            //            i--;
            //    }
            //}
            isSecond = false;
            curPlayer = 0;
        }

        public override (float,bool) DiffToBeat()
        {
            if (curPlayer < 0 || curPlayer >= competitors.Count() || resultsDetails == null || resultsDetails.Count() < 1)
            {
                return (0,false);
            }
            if (!isKO || (isKO && !isSecond))
            {
                float curJumper = Mathf.Max(fastReferenceResult[competitors[curPlayer]].points, 0);
                var best = resultsDetails.FirstOrDefault();
                if (best == null)
                {
                    return (0,false);
                }
                return (best.points - curJumper,false);
            }
            else
            {
                float curJumper = Mathf.Max(fastReferenceResult[competitors[curPlayer]].points, 0);
                return (firstResult.sum - curJumper,false);
            }
        }

        //private bool AddPlayerToStartList(ref int i,ref List<int> luckyIndexes, bool isLucky = true)
        //{
        //    if (resultsDetails.ElementAt(i).isWinInPair || (isLucky && luckyIndexes.Contains(i)))
        //    {
        //        if(!isLucky && luckyIndexes.Contains(i))
        //            return false;

        //        resultsDetails.ElementAt(i).isWinInPair = true;
        //        competitors.Add(resultsDetails.ElementAt(i).competitorObj);
        //        if (roundNumber > 1)
        //            resultsDetails.ElementAt(i).lastRoundRank = i + 1;
        //        else
        //            resultsDetails.ElementAt(i).lp = i + 1;

        //        i--;
        //        return true;
        //    }
        //    return false;
        //}

        protected override void ShowJumperInfo(int posInStartList)
        {
            if (coroutineInfoHide != null)
                StopCoroutine(coroutineInfoHide);
            sceneObjects.jumperOverlay.Hide();
            if (posInStartList != competitors.Count() - 1 || isSecond)
            {
                string nextInfo;
                if (isSecond)
                {
                    CompetitorResult result = fastReferenceResult[competitors[curPlayer-1]];
                    nextInfo = "KO: " + result.Name +" "+ firstResult.dist+"m "+ firstResult.sum;
                }
                else if(isKO)
                {
                    CompetitorResult result = fastReferenceResult[competitors[curPlayer+1]];
                    nextInfo = "KO: " + result.Name;
                }
                else
                {
                    CompetitorResult result = fastReferenceResult[competitors[curPlayer + 1]];
                    nextInfo = "NEXT: " + result.Name;
                }
                sceneObjects.jumperOverlay.Show(fastReferenceResult[competitors[posInStartList]], nextInfo);
            }
            else
                sceneObjects.jumperOverlay.Show(fastReferenceResult[competitors[posInStartList]], "");

            coroutineInfoHide = StartCoroutine(HideInfo());
        }
    }
}