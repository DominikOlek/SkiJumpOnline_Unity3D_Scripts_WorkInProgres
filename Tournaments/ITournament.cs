using Assets.Scripts.Competition.Controllers;
using Assets.Scripts.Competition.Other;
using Assets.Scripts.ImportData;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tournaments
{
    public interface ITournamenta
    {
        List<CompetitionInTournament> Competitions { get; }
        int getCurCompetitionId { get; }
        bool IsFinished { get; }
        bool IsLockable { get; }
        string Name { get; }
        List<PlayerScore> Ranking { get; }
        List<NormalTournament> TournamentsList { get; }

        void AddResults(Dictionary<Competitor, CompetitorResult> fastReferenceResult);
        void addTournament(NormalTournament tournament);
        void AwakeTick();
        void CreateReference();
        List<PlayerScore> GetRanking(int index);
        List<CompetitionInTournament> GetTournamentCompetitions(int index);
        string GetTournamentName(int index);
        bool IsAvailable();
        CompetitionInTournament NextCompetition();
        void ResetData(AIList aIList, Text nextCompetition, GameObject position, GameObject progress);
        void ResetInstance(List<CompetitionInTournament> competitions_);
        void ShowDetails(Text title, Text desc, Text time, Slider dif, Slider progress, GameObject prizeList, GameObject prizePref);
        void ShowPage(Text nextCompetition, GameObject position, GameObject progress);
        void StartTournament();
    }
}