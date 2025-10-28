using Assets.Scripts.Competition;
using Assets.Scripts.ImportData;
using Assets.Scripts.Jumping.StaticInfo;
using Assets.Scripts.SuitShop;
using Assets.Scripts.Tournaments;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    public class SingleMenu : MonoBehaviour, IConfirmPopUp
    {
        [SerializeField] JumperInfoGlobal jumperInfo;
        [SerializeField] GameObject sliderINRUN, sliderFLY, sliderLAND;
        [SerializeField] Text money;
        [SerializeField] GameObject MainMenu, Shop, Trainings, Info,Tournaments;
        [SerializeField] RawImage[] levelStars = new RawImage[3];
        [SerializeField] CreatePlayer createPlayer;
        [SerializeField] private ConfirmPopUpScript confirmPopUpScript;
        public void ShopButton()
        {
            Shop.SetActive(true);
            this.gameObject.SetActive(false);
        }


        public void Back()
        {
            SingleProgressIE.isFirst = true;
            MainMenu.SetActive(true);
            this.gameObject.SetActive(false);
        }

        public void TourButton()
        {
            Tournaments.SetActive(true);
            this.gameObject.SetActive(false);
        }

        public void TrainButton()
        {
            Trainings.SetActive(true);
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (String.Equals(jumperInfo.Name, String.Empty))
            {
                createPlayer.Show(this,jumperInfo);
            }
            else
                Show();
            Shop.GetComponent<Suits>().AwakeTick();
        }

        public void Show(bool isAfterReset = false)
        {
            money.text = jumperInfo.money.ToString("0.00");
            sliderFLY.GetComponent<Slider>().value = jumperInfo.MeanFly();
            sliderINRUN.GetComponent<Slider>().value = jumperInfo.MeanRun();
            sliderLAND.GetComponent<Slider>().value = jumperInfo.MeanDown();

            sliderFLY.GetComponentInChildren<Text>().text = "FLY " + jumperInfo.MeanFly().ToString("0.00");
            sliderINRUN.GetComponentInChildren<Text>().text = "INRUN " + jumperInfo.MeanRun().ToString("0.00");
            sliderLAND.GetComponentInChildren<Text>().text = "LANDING " + jumperInfo.MeanDown().ToString("0.00");

            int lvl = jumperInfo.getLevel();
            for (int i = 0; i < levelStars.Length; i++)
            {
                if (i < lvl)
                {
                    levelStars[i].color = Color.white;
                }
                else
                {
                    levelStars[i].color = new Color(255f, 255f, 255f, 0.6f);
                }
            }

            if(isAfterReset)
                Tournaments.GetComponent<TournamentsController>().ResetAll();
        }

        public void ResetButton()
        {
            confirmPopUpScript.Show(this);
        }

        public void Confirm()
        {
            createPlayer.Show(this, jumperInfo);
        }

        public void Reject()
        {
            return;
        }

        public string GetConfirmText()
        {
            return $"Are you sure to clear all {jumperInfo.Name} progress ?";
        }

    }
}