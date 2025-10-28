using Assets.Scripts.ImportData;
using Assets.Scripts.Menu.EventCtrl;
using System.Linq;
using UnityEngine;


namespace Assets.Scripts.Menu
{
    public class SelectableCountryList : MonoBehaviour
    {
        private CreatePlayer createPlayer;
        public GameObject countryPref;
        public GameObject contentField;
        private CountryList countryList;

        private void Start()
        {
        }

        public void Show(CreatePlayer parent)
        {
            GameObject tmp = GameObject.FindGameObjectWithTag("ImportController");
            if (tmp == null)
            {
                return;
            }
            countryList = tmp.GetComponent<CountryList>();
            foreach (Transform t in contentField.transform.Cast<Transform>().ToArray())
            {
                GameObject.Destroy(t.gameObject);
            }
            foreach (Country country in countryList.countries.Values)
            {
                GameObject instance = GameObject.Instantiate(countryPref);
                instance.GetComponent<HoverInfo>().CreateInstance(country, this, countryList);
                instance.transform.SetParent(contentField.transform);
                instance.transform.localScale = Vector3.one;
            }
            this.gameObject.SetActive(true);
            this.createPlayer = parent;
        }

        public void Selected(Country country)
        {
            this.gameObject.SetActive(false);
            createPlayer.ChangeCountry(country);
        }
    }
}