using System.Collections.Generic;
using System;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

namespace Assets.Scripts.ImportData
{
    public class CountryList : MonoBehaviour
    {
        public Dictionary<string, Country> countries;
        public Dictionary<int, Country> countriesByID;
        private static CountryList instance;
        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            string fullPath = Path.Combine(Application.streamingAssetsPath, "countries.json");
            using (StreamReader r = new StreamReader(fullPath))
            {
                string json = r.ReadToEnd();
                List<Country> list = JsonConvert.DeserializeObject<List<Country>>(json);
                countries = list.ToDictionary(a => a.alpha3);
                countriesByID = list.ToDictionary(a => a.id);
            }
        }
        /// <summary>
        /// Throws keynotfound and FileNotFoundException
        /// </summary>
        public Texture getFlag(string country)
        {
            Texture txt = Resources.Load<Texture>("Flags/" + countries[country].alpha2);
            if (txt == null)
            {
                throw new FileNotFoundException("Without flag");
            }
            return txt;
        }

        /// <summary>
        /// Throws keynotfound
        /// </summary>
        public Country getCountry(String country3digit)
        {
            return countries[country3digit];
        }
    }

    public class Country
    {
        public int id;
        public string alpha2;
        public string alpha3;
        public string name;

        public override bool Equals(object obj)
        {
            return obj is Country country &&
                   id == country.id &&
                   alpha2 == country.alpha2 &&
                   alpha3 == country.alpha3 &&
                   name == country.name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(id, alpha2, alpha3, name);
        }
    }
}