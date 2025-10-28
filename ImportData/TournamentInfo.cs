using Assets.Scripts.Competition.Other;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Assets.Scripts.ImportData
{
    public class TournamentInfo
    {
        public CompetitorDto[] competitors;
        public int nextCompetition;
        public int groupSize;
        public float[] pointsArray, incomeArray;

        public TournamentInfo() { }
        public TournamentInfo(List<PlayerScore> ranking, int nextComp, int groupSize, float[] pointsArray, float[] incomeArray)
        {
            this.nextCompetition = nextComp;
            this.pointsArray = pointsArray;
            this.incomeArray = incomeArray;
            competitors = new CompetitorDto[ranking.Count];
            int i = 0;
            foreach (PlayerScore score in ranking)
            {
                competitors[i] = new CompetitorDto { competitorID = score.CompetitorID, points = score.points };
                i++;
            }

            this.groupSize = groupSize;
        }

        public void SaveToFile(string path)
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path.Remove(path.LastIndexOf(Path.DirectorySeparatorChar)));
            }
            using (StreamWriter w = new StreamWriter(path))
            {
                w.Write(json);
            }
        }
    }

    public class CompetitorDto
    {
        public float points;
        public int competitorID;
    }
}