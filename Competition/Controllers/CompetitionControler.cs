using Assets.Scripts.Competition.Other;
using Assets.Scripts.ImportData;
using Assets.Scripts.Jumping;
using Assets.Scripts.Jumping.StaticInfo;
using Assets.Scripts.Jumping.UI;
using Assets.Scripts.SuitShop;
using Assets.Scripts.WebDTO;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Cinemachine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

namespace Assets.Scripts.Competition.Controllers {

    [RequireComponent(typeof(SkipRound))]
    public class StandardCompetition : ICompetitionControler
    {

        public override void SetStartList(List<Competitor> list,int difficulty)
        {
            this.groupSize = competitionSettings.groupSize;
            this.currentGroup = 1;
            this.difficulty = difficulty;
            competitors = list;
            //startList = list;
            resultsDetails = new List<CompetitorResult>();
            fastReferenceResult = new Dictionary<Competitor, CompetitorResult>();
            for(int i =0;i<competitors.Count();i++)
            {
                fastReferenceResult.Add(competitors[i], new CompetitorResult(i,competitors[i],i+1,i+1));
                resultsDetails.Add(fastReferenceResult[competitors[i]]); 
            }
            leader = competitors[competitors.Count()-1];
        }

        public override void SetResult(PointsStat stats, bool isShowResult = true)
        {
            base.SetResult(stats,isShowResult);
            CompetitorResult result = fastReferenceResult[competitors[curPlayer-1]];

            //float resultFAST = -2.758f * stats.windX - 7.299f * stats.windY + 349.063f * aIJumperInfo.controlFactorRun
            //    + 165.76f * aIJumperInfo.controlFactorFly - 181.186f * aIJumperInfo.controlFactorDown + 88.525f * aIJumperInfo.swingsFactorRun +
            //    130.369f * aIJumperInfo.swingsFactorFly - 255.985f * aIJumperInfo.swingsFactorDown + 283.296f * aIJumper.lubricationFactor +
            //    172.554f * aIJumperInfo.suitFactor + 102.497f * aIJumperInfo.bootsFactor - 723.806f;

            //float resultFAST2 = 2.134f * stats.windX - 7.327f * stats.windY + 161.050f * aIJumperInfo.controlFactorRun
            //    + 97.146f * aIJumperInfo.controlFactorFly - 100.257f * aIJumperInfo.controlFactorDown + -31.888f * aIJumperInfo.swingsFactorRun +
            //    171.896f * aIJumperInfo.swingsFactorFly - 80.578f * aIJumperInfo.swingsFactorDown + 266.186f * aIJumper.lubricationFactor +
            //    182.002f * aIJumperInfo.suitFactor + 154.888f * aIJumperInfo.bootsFactor - 686.480f;

            //Debug.Log(resultFAST/2 +" lub "+ resultFAST2 / 2 + " a jest "+ stats.sum);

            //if (roundNumber == 2)
            //    tOCSVs.Add(new TOCSV
            //    {
            //        bootsFactor = aIJumperInfo.bootsFactor,
            //        controlFactorDown = aIJumperInfo.controlFactorDown,
            //        controlFactorFly = aIJumperInfo.controlFactorFly,
            //        controlFactorRun = aIJumperInfo.controlFactorRun,
            //        lubricationFactor = aIJumper.lubricationFactor,
            //        result = result.points,
            //        swingsFactorRun = aIJumperInfo.swingsFactorRun,
            //        suitFactor = aIJumperInfo.suitFactor,
            //        swingsFactorDown = aIJumperInfo.swingsFactorDown,
            //        swingsFactorFly = aIJumperInfo.swingsFactorFly,
            //        windx = result.windx,
            //        windy = result.windy
            //    });

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
                        if(roundNumber< competitionSettings.roundSetting.Length && resultsDetails[i].rank <= competitionSettings.roundSetting[roundNumber].competitorsInRound)
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
                sceneObjects.jumpResult.ShowStat(stats, result);
                coroutineNextJump = StartCoroutine(StartNext());
            }
        }

        public override void CreateStartList()
        {
            int competitorsNumber = Mathf.Min(competitionSettings.roundSetting[roundNumber - 1].competitorsInRound - 1, competitors.Count() - 1);
            int i = competitorsNumber;
            competitors.Clear();
            while (i >= 0 && resultsDetails.ElementAt(i).isQualified)
            {
                competitors.Add(resultsDetails.ElementAt(i).competitorObj);
                if (roundNumber > 1)
                    resultsDetails.ElementAt(i).lastRoundRank = i + 1;
                else
                    resultsDetails.ElementAt(i).lp = competitorsNumber - i + 1;
                i--;
            }
            curPlayer = 0;
        }
    }

   

    //public class TOCSV
    //{
    //    public float controlFactorRun = 1;
    //    public float swingsFactorRun = 1;
    //    public float lubricationFactor = 1;

    //    public float controlFactorFly = 1;
    //    public float swingsFactorFly = 1;
    //    public float suitFactor = 1;

    //    public float controlFactorDown = 1;
    //    public float swingsFactorDown = 1;
    //    public float bootsFactor = 1;

    //    public float result = 1;
    //    public float windx = 1;
    //    public float windy = 1;
//
}