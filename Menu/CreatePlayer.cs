using Assets.Scripts.ImportData;
using Assets.Scripts.Jumping.StaticInfo;
using Assets.Scripts.Menu;
using Assets.Scripts.SuitShop;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts.Menu
{

    public class CreatePlayer : MonoBehaviour
    {
        [SerializeField] Equipment equipment;
        [SerializeField] float startMoney, stepSize, moneyChange;
        private SingleMenu menu;
        JumperInfoGlobal skiInfo;
        [SerializeField] GameObject INRUNSWING, INRUNCTRL, FLYSWING, FLYCTRL, LANDSWING, LANDCTRL;
        [SerializeField] Text money;
        [SerializeField] InputField nameField;
        private Country selectedCountry;
        [SerializeField] SelectableCountryList coutriesList;
        [SerializeField] RawImage countryButton;
        private CountryList countryList;
        private void Start()
        {
        }
        public void Show(SingleMenu menu, JumperInfoGlobal skiInfo)
        {
            GameObject tmp = GameObject.FindGameObjectWithTag("ImportController");
            if (tmp == null)
            {
                return;
            }
            countryList = tmp.GetComponent<CountryList>();
            this.menu = menu;
            this.skiInfo = skiInfo;
            this.gameObject.SetActive(true);
            Slider[] all = this.GetComponentsInChildren<Slider>();
            skiInfo.swingsFactorDown = JumperInfoGlobal.MINFACTOR;
            skiInfo.swingsFactorFly = JumperInfoGlobal.MINFACTOR;
            skiInfo.swingsFactorRun = JumperInfoGlobal.MINFACTOR;
            skiInfo.controlFactorRun = JumperInfoGlobal.MINFACTOR;
            skiInfo.controlFactorFly = JumperInfoGlobal.MINFACTOR;
            skiInfo.controlFactorDown = JumperInfoGlobal.MINFACTOR;
            skiInfo.money = startMoney;
            skiInfo.winLevel = 0;
            equipment.ResetOwner();
            equipment.ImportClothe(new Dictionary<SuitEnum, int> {
            {SuitEnum.Ski,0 },
            {SuitEnum.Suit,0},
            {SuitEnum.Boots,0},
            {SuitEnum.Gloves,0},
            {SuitEnum.Helmet,0},
            {SuitEnum.Front,0},
        });
            money.text = "MONEY " + startMoney.ToString("0.00");

            foreach (Slider s in all)
            {
                s.minValue = JumperInfoGlobal.MINFACTOR;
                s.maxValue = JumperInfoGlobal.MAXFACTOR;
            }
            INRUNSWING.GetComponentInChildren<Text>().text = "INRUN SWING " + skiInfo.swingsFactorRun;
            INRUNCTRL.GetComponentInChildren<Text>().text = "INRUN CONTROL " + skiInfo.controlFactorRun;
            FLYSWING.GetComponentInChildren<Text>().text = "FLY SWING " + skiInfo.swingsFactorFly;
            FLYCTRL.GetComponentInChildren<Text>().text = "FLY CONTROL " + skiInfo.controlFactorFly;
            LANDSWING.GetComponentInChildren<Text>().text = "LAND SWING " + skiInfo.swingsFactorDown;
            LANDCTRL.GetComponentInChildren<Text>().text = "LAND CONTROL " + skiInfo.controlFactorDown;
            INRUNSWING.GetComponentInChildren<Slider>().value = skiInfo.swingsFactorRun;
            INRUNCTRL.GetComponentInChildren<Slider>().value = skiInfo.controlFactorRun;
            FLYSWING.GetComponentInChildren<Slider>().value = skiInfo.swingsFactorFly;
            FLYCTRL.GetComponentInChildren<Slider>().value = skiInfo.controlFactorFly;
            LANDSWING.GetComponentInChildren<Slider>().value = skiInfo.swingsFactorDown;
            LANDCTRL.GetComponentInChildren<Slider>().value = skiInfo.controlFactorDown;
            if (skiInfo.Country3dig.Length == 3)
                selectedCountry = countryList.getCountry(skiInfo.Country3dig);
            else
                selectedCountry = countryList.getCountry("pol");

            nameField.text = skiInfo.Name;

            try
            {
                countryButton.texture = countryList.getFlag(selectedCountry.alpha3);
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is FileNotFoundException)
            {
                Debug.LogError(ex.Message);
            }
        }


        public void AddInRunSwing(GameObject parent)
        {
            skiInfo.swingsFactorRun = AddFactor(parent, "INRUN SWING ");
        }
        public void MinusInRunSwing(GameObject parent)
        {
            skiInfo.swingsFactorRun = MinusFactor(parent, "INRUN SWING ", skiInfo.swingsFactorRun);
        }
        public void AddFlySwing(GameObject parent)
        {
            skiInfo.swingsFactorFly = AddFactor(parent, "FLY SWING ");
        }
        public void MinusFlySwing(GameObject parent)
        {
            skiInfo.swingsFactorFly = MinusFactor(parent, "FLY SWING ", skiInfo.swingsFactorFly);
        }
        public void AddLandSwing(GameObject parent)
        {
            skiInfo.swingsFactorDown = AddFactor(parent, "LAND SWING ");
        }
        public void MinusLandSwing(GameObject parent)
        {
            skiInfo.swingsFactorDown = MinusFactor(parent, "LAND SWING ", skiInfo.swingsFactorDown);
        }
        public void AddFlyCTRL(GameObject parent)
        {
            skiInfo.controlFactorFly = AddFactor(parent, "FLY CONTROL ");
        }
        public void MinusFlyCTRL(GameObject parent)
        {
            skiInfo.controlFactorFly = MinusFactor(parent, "FLY CONTROL ", skiInfo.controlFactorFly);
        }
        public void AddInRunCTRL(GameObject parent)
        {
            skiInfo.controlFactorRun = AddFactor(parent, "INRUN CONTROL ");
        }
        public void MinusInRunCTRL(GameObject parent)
        {
            skiInfo.controlFactorRun = MinusFactor(parent, "INRUN CONTROL ", skiInfo.controlFactorRun);
        }
        public void AddLandCTRL(GameObject parent)
        {
            skiInfo.controlFactorDown = AddFactor(parent, "LAND CONTROL ");
        }
        public void MinusLandCTRL(GameObject parent)
        {
            skiInfo.controlFactorDown = MinusFactor(parent, "LAND CONTROL ", skiInfo.controlFactorDown);
        }

        public void CreateButton()
        {
            if (nameField.text.Length < 3)
                return;
            skiInfo.Name = nameField.text;
            skiInfo.Country3dig = selectedCountry.alpha3;
            this.gameObject.SetActive(false);
            menu.Show(true);
        }

        public void ChangeCountry(Country country)
        {
            selectedCountry = country;
            try
            {
                countryButton.texture = countryList.getFlag(selectedCountry.alpha3);
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is FileNotFoundException)
            {
                Debug.LogError(ex.Message);
            }
            skiInfo.Country3dig = country.alpha3;
        }
        private float AddFactor(GameObject parent, string textAbove)
        {
            Slider slider = parent.GetComponentInChildren<Slider>();
            if (skiInfo.money >= moneyChange)
            {
                slider.value += stepSize;
                Text text = parent.GetComponentInChildren<Text>();
                text.text = textAbove + slider.value.ToString("0.00");
                skiInfo.money -= moneyChange;
                money.text = "MONEY " + skiInfo.money.ToString("0.00");
            }
            return slider.value;
        }

        private float MinusFactor(GameObject parent, string textAbove, float curVal)
        {
            Slider slider = parent.GetComponentInChildren<Slider>();
            if (curVal > JumperInfoGlobal.MINFACTOR)
            {
                slider.value -= stepSize;
                Text text = parent.GetComponentInChildren<Text>();
                text.text = textAbove + slider.value.ToString("0.00");
                skiInfo.money += moneyChange;
                money.text = "MONEY " + skiInfo.money.ToString("0.00");
            }
            return slider.value;
        }

        public void ShowCountriesList()
        {
            coutriesList.Show(this);
        }

    }
}