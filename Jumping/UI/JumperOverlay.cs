using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Competition;
using Assets.Scripts.ImportData;
using Assets.Scripts.Competition.Controllers;

namespace Assets.Scripts.Jumping.UI
{

    public class JumperOverlay : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI nameField, dist, lp, rank, points, next;
        [SerializeField] RawImage country;
        private CountryList countryList;

        public void Show(Competitor competitor,int attempt = 1)
        {
            if (countryList == null)
                countryList = GameObject.Find("ImportController").GetComponent<CountryList>();

            nameField.text = competitor.getName();
            lp.text = attempt.ToString();
            rank.text = "";
            points.text = "";
            dist.text = "";
            next.text = "";
            try
            {
                country.texture = countryList.getFlag(competitor.getCountry());
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is FileNotFoundException)
            {
                Debug.LogError(ex.Message);
            }
            this.gameObject.SetActive(true);
        }

        public void Show(CompetitorResult info, String nextValue)
        {
            if(countryList == null)
                countryList = GameObject.Find("ImportController").GetComponent<CountryList>();

            nameField.text = info.Name;
            lp.text = info.lp.ToString();
            if (info.lastRoundRank != -1)
            {
                rank.text = info.lastRoundRank.ToString();
                if(info.points>0) points.text = info.points.ToString("0.00");
                else points.text = "";

                if (info.distances.Count() > 0) dist.text = info.distances.Last().ToString("0.00") + " m";
                else dist.text = "";
            }
            else
            {
                rank.text = "";
                points.text = "";
                dist.text = "";
            }
            try
            {
                country.texture = countryList.getFlag(info.Country());
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is FileNotFoundException)
            {
                Debug.LogError(ex.Message);
            }
            next.text = nextValue;
            this.gameObject.SetActive(true);
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}