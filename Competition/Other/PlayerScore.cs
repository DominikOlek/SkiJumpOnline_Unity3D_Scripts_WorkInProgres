using Assets.Scripts.ImportData;
using Assets.Scripts.Jumping.StaticInfo;
using System.Linq;
using System;
using UnityEngine;
using Assets.Scripts.Competition.Competitors;

namespace Assets.Scripts.Competition.Other
{
    [Serializable]
    public class PlayerScore : IComparable<PlayerScore>
    {
        public float points;
        private int competitorID;
        public int CompetitorID => competitorID;
        private Competitor competitor;
        private int groupSize;
        public int GroupSize => groupSize;

        public PlayerScore(float points, int competitorID, Competitor competitor)
        {
            this.points = points;
            this.competitorID = competitorID;
            this.competitor = competitor;
        }
        public PlayerScore(float points, int competitorID, Competitor competitor, int groupSize) : this(points, competitorID, competitor)
        {
            this.groupSize = groupSize;
        }

        public void createReference(AIList aIList, JumperInfoGlobal jumper, CountryList countryList, int[] difficulty, int groupSize)
        {
            if (competitor == null || groupSize != this.groupSize)
                if (groupSize == 1)
                {
                    competitor = competitorID == 0 ? jumper : aIList.AIJumpers[competitorID];
                    this.groupSize = groupSize;
                }
                else
                {
                    try
                    {
                        Country country = countryList.countriesByID[competitorID];
                        competitor = new GroupCompetitor(
                            groupSize: groupSize,
                            country: country,
                            diff: difficulty.Max(),
                            aIList: aIList,
                            jumperInfo: jumper
                        );
                        this.groupSize = groupSize;
                    }
                    catch (MissingComponentException)
                    {
                        return;
                    }
                }
        }

        public int CompareTo(PlayerScore other)
        {
            if (other.points == points)
                return (int)Mathf.Sign(other.competitorID - competitorID); ;
            return (int)Mathf.Sign(other.points - points);
        }

        public Competitor GetCompetitor()
        {
            return competitor;
        }

    }
}