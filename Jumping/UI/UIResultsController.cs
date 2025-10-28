using Assets.Scripts.Competition;
using Assets.Scripts.Competition.Controllers;
using Assets.Scripts.ImportData;
using Assets.Scripts.Jumping.StaticInfo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Jumping.UI
{

    public class UIResultsController : MonoBehaviour
    {
        [SerializeField] SkiJumpInfo jumpInfo;
        [SerializeField] GameObject overlay,nextRoundButton;
        [SerializeField] TextMeshProUGUI[] distances;
        [SerializeField] TextMeshProUGUI positions, points, names,header,description;
        [SerializeField] GameObject flags;
        private CountryList countryList;

        List<RawImage> images = new List<RawImage>();
        List<CompetitorResult> resultsList;
        int pageSize = 10;
        int curPage = 0;
        int distOffset = 0;

        private void Awake()
        {
            countryList = GameObject.Find("ImportController").GetComponent<CountryList>();
        }

        private void Start()
        {
            for (int i = 0; i < flags.transform.childCount; i++)
            {
                images.Add(flags.transform.GetChild(i).GetComponent<RawImage>());
            }
        }
        public void ShowResults(List<CompetitorResult> results,int roundNumber)
        {
            resultsList = results;
            curPage = 0;
            SetData();
            string tmp = GameObject.FindGameObjectWithTag("CompetitionController").GetComponent<ICompetitionControler>().shortDescription;
            description.text = jumpInfo.skiJumpInfo.name + " "+tmp;
            header.text = Competition.CompetitionSettings.ROUNDNAME[roundNumber] +" RESULTS";
            overlay.SetActive(true);
        }

        public void HideResults()
        {
            overlay.SetActive(false);
        }

        private void SetData()
        {
            positions.text = string.Empty;
            points.text = string.Empty;
            names.text = string.Empty;
            foreach (var d in distances)
                d.text = string.Empty;
            foreach (var f in images)
                f.enabled = false;

            CompetitorResult result = null;
            for (int i = curPage * pageSize; i < (curPage + 1) * pageSize; i++)
            {
                if (i == resultsList.Count())
                    break;
                result = resultsList[i];
                positions.text += (!result.isQualified ? result.rank : (result.isWinInPair ? "<color=green>" + result.rank + "</color>" : "<color=orange>" + result.rank + "</color>")) + "\n";
                //positions.text += result.rank + "\n";
                names.text += (result.competitorObj.isAI() ? result.Name : "<color=#6346D1>" + result.Name+"</color>" ) + "\n";
                points.text += result.points.ToString("0.00") + "\n";
                //int j = 0;
                //foreach (var d in result.distances)
                //{
                //    distances[j++].text += d.ToString("0.00") + "\n";
                //    if (j >= distances.Length)
                //        break;
                //}

                int j = 0;
                for(int k = Mathf.Max(0, result.distances.Count() - distances.Length + distOffset); k< result.distances.Count() + distOffset; k++)
                    distances[j++].text += result.distances[k].ToString("0.00") + "\n";

                try
                {
                    images[i - curPage * pageSize].texture = countryList.getFlag(result.Country());
                    images[i - curPage * pageSize].enabled = true;
                }
                catch (Exception ex) when (ex is KeyNotFoundException || ex is FileNotFoundException)
                {
                    Debug.LogError(ex.Message);
                }
            }
        }

        public void nextPage()
        {
            if (curPage < (resultsList.Count() - 1) / pageSize)
            {
                curPage++;
                SetData();
            }
        }

        public void prevPage()
        {
            if (curPage > 0)
            {
                curPage--;
                SetData();
            }
        }

        public void nextDist()
        {
            if (distOffset < 0)
            {
                distOffset++;
                SetData();
            }
        }

        public void prevDist()
        {
            if (resultsList[0].distances.Count() > distances.Length && resultsList[0].distances.Count() - distances.Length > -distOffset)
            {
                distOffset--;
                SetData();
            }
        }

        public void SetNextRoundButton(string text,UnityAction call = null)
        {
            nextRoundButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = text;
            if (call != null)
                nextRoundButton.GetComponent<Button>().onClick.AddListener(call);
        }




    }
}
