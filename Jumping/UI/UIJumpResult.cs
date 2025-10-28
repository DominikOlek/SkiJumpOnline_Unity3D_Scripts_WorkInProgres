using Assets.Scripts.Competition;
using Assets.Scripts.Competition.Controllers;
using Assets.Scripts.ImportData;
using Assets.Scripts.WebDTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Jumping.UI
{
    public class UiJumpResult : MonoBehaviour
    {
        [SerializeField] GameObject overlay;
        [SerializeField] TextMeshProUGUI[] judges = new TextMeshProUGUI[5];
        [SerializeField] TextMeshProUGUI sum, wind, nameField, country, rank, lp;
        [SerializeField] RawImage flag;
        [SerializeField] TextMeshProUGUI[] distances;
        private CountryList countryList;
        bool minn = false, maxx = false;

        private void Awake()
        {
            countryList = GameObject.Find("ImportController").GetComponent<CountryList>();
        }

        public void ShowStat(Competitor competitor, PointsStat stat,string resultText = "",int attempt = 1)
        {
            overlay.SetActive(true);
            setJudge(stat.pointsJury);
            this.wind.text = stat.wind.ToString("0.00");
            distances[0].text = stat.dist.ToString("0.00") + " m";
            distances[1].text = resultText;
            nameField.text = competitor.getName();

            setCountry(competitor.getCountry());
            rank.text = "1";
            lp.text = attempt.ToString();
            sum.text = stat.sum.ToString("0.00");
        }

        private void setJudge(float[] pointsJury) {
            for (int i = 0; i < 5; i++)
            {
                judges[i].text = pointsJury[i].ToString();
                if ((pointsJury[i] == pointsJury.Min() && !minn))
                {
                    minn = true;
                    judges[i].color = Color.gray;
                }
                if (pointsJury[i] == pointsJury.Max() && !maxx)
                {
                    maxx = true;
                    judges[i].color = Color.gray;
                }
            }
        }

        private void setCountry(string country3Dig)
        {
            try
            {
                country.text = countryList.getCountry(country3Dig).name.ToUpper();
            }
            catch (KeyNotFoundException ex)
            {
                Debug.LogError(ex.Message);
            }

            try
            {
                flag.texture = countryList.getFlag(country3Dig);
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is FileNotFoundException)
            {
                Debug.LogError(ex.Message);
            }
        }

        public void ShowStat(float[] pointsJury, float wind, float dist, float distPoints, float sumP, CompetitorResult allResult)
        {
            overlay.SetActive(true);
            setJudge(pointsJury);
            this.wind.text = wind.ToString("0.00");
            foreach(var d in distances)
            {
                d.text = "";
            }

            int j = 0;
            for (int k = Mathf.Max(0, allResult.distances.Count() - distances.Length); k < allResult.distances.Count(); k++)
                distances[j++].text = allResult.distances[k].ToString("0.00") + " m";

            //int j = 0;
            //foreach (var d in allResult.distances)
            //{
            //    if (j >= distances.Length)
            //        break;
            //    distances[j++].text = d.ToString("0.00") + " m";
            //}
            nameField.text = allResult.Name;
            setCountry(allResult.Country());
            rank.text = allResult.rank.ToString();
            lp.text = allResult.lp.ToString();
            sum.text = allResult.points.ToString("0.00");
        }

        public void ShowStat(PointsStat stat, CompetitorResult allResult)
        {
            ShowStat(stat.pointsJury, stat.wind, stat.dist, stat.distPoints, stat.sum, allResult);
        }

        public void HideStat()
        {
            overlay.SetActive(false);
            minn = false;
            maxx = false;
            foreach (TextMeshProUGUI item in judges)
            {
                item.color = Color.black;
            }
        }
    }
}