using Assets.Scripts.ImportData;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace Assets.Scripts.Menu.EventCtrl
{
    public class HoverInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private Country country;
        private SelectableCountryList parent;
        private CountryList countryList;

        public void CreateInstance(Country country, SelectableCountryList parent, CountryList countryList)
        {
            this.country = country;
            this.parent = parent;
            this.countryList = countryList;
            GetComponentInChildren<Text>(true).text = country.name;
            try
            {
                GetComponent<RawImage>().texture = countryList.getFlag(country.alpha3);
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is FileNotFoundException)
            {
                Debug.LogError(ex.Message);
            }
        }

        private void Start()
        {
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (parent != null)
                parent.Selected(country);
        }
    }
}