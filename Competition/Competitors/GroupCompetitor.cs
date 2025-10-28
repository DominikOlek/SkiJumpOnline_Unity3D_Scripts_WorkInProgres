using Assets.Scripts.ImportData;
using Assets.Scripts.Jumping.StaticInfo;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Competition.Competitors
{
    public class GroupCompetitor : Competitor
    {
        private int groupSize;
        private List<Competitor> competitors;
        private string country;
        private string name;
        private int id;
        private bool isAi = true;
        private int curID = 0;

        public GroupCompetitor(int groupSize, Country country, int diff, AIList aIList, JumperInfoGlobal jumperInfo)
        {
            competitors = new List<Competitor>();
            name = country.name;
            this.country = country.alpha3;
            List<AIInfo> all;
            this.groupSize = groupSize;
            int tmp = groupSize;
            if (jumperInfo.getCountry() == country.alpha3)
            {
                tmp--;
                isAi = false;
                competitors.Add(jumperInfo);
            }
            if (aIList.JumpersByCountry.TryGetValue(country.id, out all))
            {
                competitors.AddRange(all.Where(a => a.Level == diff).OrderByDescending(a => a.avgStats()).Take(tmp));
            }
            if (all != null && competitors.Count() != groupSize)
            {
                competitors.AddRange(all.Where(a => a.Level < diff).OrderByDescending(a => a.avgStats()).Take(groupSize - competitors.Count()));
            }
            //if (all != null && competitors.Count() != groupSize)
            //{
            //    competitors.AddRange(all.Where(a => a.Level > diff).OrderByDescending(a => a.avgStats()).Take(groupSize - competitors.Count()));
            //}
            if (competitors.Count() < groupSize)
            {
                throw new MissingComponentException("Too few jumpers to create this group");
            }
            curID = 0;
        }

        public override bool Equals(object obj)
        {
            return obj is GroupCompetitor competitor &&
                   country == competitor.country &&
                   name == competitor.name &&
                   id == competitor.id &&
                   isAi == competitor.isAi
                   ;
        }

        public int getAiId()
        {
            return 0;
        }

        public string getCountry()
        {
            return country;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(country, name, id, isAi);
        }

        public string getName()
        {
            return name;
        }

        public Competitor GetNextCompetitor()
        {
            Debug.Log(country);
            Debug.Log(curID);
            Debug.Log(competitors.Count());
            if (curID >= competitors.Count())
                curID = 0;
            return competitors[curID++].GetNextCompetitor();
        }

        public bool isAI()
        {
            return isAi;
        }

    }
}