using Assets.Scripts.Competition;
using Assets.Scripts.ImportData;
using Assets.Scripts.Jumping.StaticInfo;
using Assets.Scripts.Menu;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Tournaments
{
    public class TournamentsController : MonoBehaviour
    {
        [SerializeField] public List<NormalTournament> tournaments;
        [SerializeField] private GameObject tourList,buttonTourPref,textPref,prizeList,singleMenu;
        [SerializeField] private Text titleTxt, descriptTxt, timeTxt;
        [SerializeField] private Slider difSlid, progSlid;
        [SerializeField] private Button selectButton;
        [SerializeField] private AIList aiList;
        [SerializeField] private SingleProgressIE progressExporter;

        private OneTournamentShow oneTournamentShow;
        private GameObject singleInfo, listInfo;

        private void Start()
        {
        }

        private void Awake()
        {
            singleInfo = transform.Find("OneSelected").gameObject;
            listInfo = transform.Find("Main").gameObject;
            oneTournamentShow = singleInfo.GetComponent<OneTournamentShow>();
            GameObject tmp = GameObject.FindGameObjectWithTag("ImportController");
            if (tmp == null)
            {
                return;
            }
            aiList = tmp.GetComponent<AIList>();
            foreach (var item in tournaments) {
                item.AwakeTick();
            }
            if(SceneManager.GetActiveScene().buildIndex == 0)
            {
                if (progressExporter != null)
                {
                    progressExporter.ExportData();
                }
            }
        }

        private void OnEnable()
        {
            ShowList();
        }

        public void ShowList()
        {
            foreach (Transform child in tourList.transform)
            {
                Destroy(child.gameObject);
            }
            int i = 0;
            foreach (var item in tournaments) {
                GameObject tmp = Instantiate(buttonTourPref);
                tmp.transform.SetParent(tourList.transform,false);
                tmp.transform.localScale = Vector3.one;
                tmp.transform.GetChild(0).GetComponent<Text>().text = item.Name;
                tmp.GetComponent<Button>().onClick.AddListener(() => { ShowInfo(item); });

                if(item.ranking == null || item.ranking.Count() == 0)
                {
                    item.StartTournament();
                }
                if(i==0)
                    ShowInfo(item);
                i++;
            }
        }

        public void ShowInfo(NormalTournament tournament)
        {
            selectButton.onClick.RemoveAllListeners();
            if (tournament.IsAvailable())
            {
                selectButton.onClick.AddListener(() => { NextButton(tournament); });
            }
            tournament.ShowDetails(titleTxt,descriptTxt,timeTxt,difSlid,progSlid,prizeList, textPref);
        }

        public void BackButton()
        {
            singleMenu.SetActive(true);
            this.gameObject.SetActive(false);
        }

        public void NextButton(NormalTournament tournament)
        {
            if(oneTournamentShow == null)
            {
                this.gameObject.SetActive(true);
                singleInfo = transform.Find("OneSelected").gameObject;
                listInfo = transform.Find("Main").gameObject;
                oneTournamentShow = singleInfo.GetComponent<OneTournamentShow>();
            }
            oneTournamentShow.tournament = tournament;
            singleInfo.SetActive(true);
            listInfo.SetActive(false);
        }

        public void ResetAll()
        {
            foreach(NormalTournament t in tournaments)
            {
                t.StartTournament();
            }
        }
    }
}