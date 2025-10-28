using Assets.Scripts.Competition.Other;
using Assets.Scripts.ImportData;
using Assets.Scripts.Jumping.StaticInfo;
using Assets.Scripts.Tournaments;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;

namespace Assets.Scripts.Menu
{
    public class OneTournamentShow : MonoBehaviour, IConfirmPopUp
    {
        [HideInInspector] public NormalTournament tournament;
        [SerializeField] private Text header,info,rankingName,competitionsName;
        private CountryList countryList;
        [SerializeField] private GameObject posSlider,progSlider,tournamentsMenu;
        [SerializeField] private GameObject rankingOverlay,rankingList,rankingRowPref;
        [SerializeField] private GameObject competitionsOverlay, competitionsList, competitionsRowPref;
        [SerializeField] private LoadingScreen loadingScreen;
        [SerializeField] private ConfirmPopUpScript confirmPopUpScript;
        private int rankingIndex = 0;
        private int competitionsIndex = 0;
        AIList aiList;
        public JumperInfoGlobal jumper;

        private void Awake()
        {
            GameObject tmp = GameObject.FindGameObjectWithTag("ImportController");
            if(tmp == null)
            {
                Debug.LogError("Not found ImportController in OneTournamentShow");
            }
            countryList = tmp.GetComponent<CountryList>();
        }

        private void Start()
        {
            aiList = GameObject.FindWithTag("ImportController").GetComponent<AIList>();
        }

        private void OnEnable()
        {
            header.text = tournament.Name;
            StartCoroutine(LoadStatistics());
            tournament.CreateReference(true);
        }

        public void ShowRanking()
        {
            rankingIndex = 0;
            rankingOverlay.SetActive(true);
            GenerateRanking();
        }
        public void HideRanking()
        {
            rankingOverlay.SetActive(false);
        }

        public void NextRanking() { ++rankingIndex; GenerateRanking(); }
        public void PrevRanking() { --rankingIndex; GenerateRanking(); }

        private void GenerateRanking()
        {
            tournament.CreateReference(true);
            if (rankingIndex < 0) rankingIndex = tournament.tournamentsList.Count();
            if(rankingIndex > tournament.tournamentsList.Count()) rankingIndex = 0;

            rankingName.text = tournament.GetTournamentName(rankingIndex);
            foreach (Transform child in rankingList.transform)
            {
                Destroy(child.gameObject);
            }
            int i = 1;
            var ranking = tournament.GetRanking(rankingIndex);
            foreach (var item in ranking) {
                if (item.GetCompetitor() == null)
                    continue;
                var row = UnityEngine.Object.Instantiate(rankingRowPref);
                row.transform.SetParent(rankingList.transform, false);
                row.transform.localScale = Vector3.one;
                try
                {
                    row.transform.Find("NR").GetComponent<Text>().text = i.ToString();
                    row.transform.Find("NAME").GetComponent<Text>().text = item.GetCompetitor().getName();
                    row.transform.Find("POINTS").GetComponent<Text>().text = item.points.ToString("0.00");
                    row.transform.Find("LOSETO").GetComponent<Text>().text = (ranking.First().points - item.points).ToString("0.00");
                    row.transform.Find("FLAG").GetComponent<RawImage>().texture = countryList.getFlag(item.GetCompetitor().getCountry());

                    if(!item.GetCompetitor().isAI())
                        row.transform.Find("NAME").GetComponent<Text>().color = Color.red;
                }
                catch (Exception ex) when (ex is KeyNotFoundException || ex is FileNotFoundException)
                {
                    Debug.LogError(ex.Message);
                }
                i++;
            }
        }

        public void ShowCompetitions()
        {
            competitionsIndex = 0;
            competitionsOverlay.SetActive(true);
            GenerateCompetitionList();
        }
        public void HideCompetitions()
        {
            competitionsOverlay.SetActive(false);
        }

        public void NextCompetitions() { ++competitionsIndex; GenerateCompetitionList(); }
        public void PrevCompetitions() { --competitionsIndex; GenerateCompetitionList(); }

        private void GenerateCompetitionList()
        {
            if (competitionsIndex < 0) competitionsIndex = tournament.tournamentsList.Count();
            if (competitionsIndex > tournament.tournamentsList.Count()) competitionsIndex = 0;

            competitionsName.text = tournament.GetTournamentName(competitionsIndex);
            foreach (Transform child in competitionsList.transform)
            {
                Destroy(child.gameObject);
            }
            int i = 0;
            var ranking = tournament.GetTournamentCompetitions(competitionsIndex);
            foreach (var item in ranking)
            {
                var row = UnityEngine.Object.Instantiate(competitionsRowPref);
                row.transform.SetParent(competitionsList.transform, false);
                row.transform.localScale = Vector3.one;
                try
                {
                    row.transform.Find("NR").GetComponent<Text>().text = (i+1).ToString();
                    row.transform.Find("NAME").GetComponent<Text>().text = item.Name;
                    string tours = "";
                    if(item.extraTournaments.Count() == 0 || tournament.isGroup == item.extraTournaments[0].isGroup)
                        tours = $"<color=#{UnityEngine.ColorUtility.ToHtmlStringRGB(tournament.color)}>" + tournament.Name[0] + "</color> ";
                    foreach(var t in item.extraTournaments)
                    {
                        tours += $"<color=#{UnityEngine.ColorUtility.ToHtmlStringRGB(t.color)}>" + t.Name[0] +"</color> ";
                    }
                    row.transform.Find("HS").GetComponent<Text>().text = tours;
                    row.transform.Find("RECORD").GetComponent<Text>().text = item.Record.ToString();
                    row.transform.Find("FLAG").GetComponent<RawImage>().texture = countryList.getFlag(item.Country.ToLower());

                    if (i == tournament.getCurCompetitionId)
                        row.transform.Find("NAME").GetComponent<Text>().color = Color.red;
                }
                catch (Exception ex) when (ex is KeyNotFoundException || ex is FileNotFoundException)
                {
                    Debug.LogError(ex.Message);
                }
                i++;
            }
        }

        public void Back()
        {
            tournamentsMenu.SetActive(true);
            this.gameObject.SetActive(false);
        }

        public void Play()
        {
            StartCoroutine(PlayWithLoading());
        }

        IEnumerator PlayWithLoading()
        {
            if (tournament.isFinished)
                tournament.StartTournament();

            CompetitionInTournament competition = tournament.NextCompetition();

            loadingScreen.Show(competition.skiJump);

            yield return null;

            AsyncOperation loading = SceneManager.LoadSceneAsync(competition.Name, LoadSceneMode.Single);
            loading.allowSceneActivation = false;
            yield return StartCoroutine(LoadSkiJumpAsync(loading));
        }

        public void ResetButton()
        {
            confirmPopUpScript.Show(this);
        }

        public void Confirm()
        {
            tournament.ResetData(aiList, info, posSlider, progSlider);
        }

        public void Reject()
        {
            return;
        }

        public string GetConfirmText()
        {
            return $"Are you sure to clear all progress in {tournament.Name} tournament ?";
        }

        System.Collections.IEnumerator LoadSkiJumpAsync(AsyncOperation loading)
        {
            float minLoadTime = 3f;
            float timer = 0f;

            while (!loading.isDone) {
                loadingScreen.progressSlider.value = Mathf.Clamp01(loading.progress / 0.9f);
                timer += Time.deltaTime;
                if (loading.progress >= 0.9f && timer >= minLoadTime)
                {
                    loading.allowSceneActivation = true;
                }

                yield return null;
            }
        }

        System.Collections.IEnumerator LoadStatistics() {
            yield return new WaitForSeconds(0.2f);
            tournament.ShowPage(info, posSlider, progSlider);
        }
    }
}