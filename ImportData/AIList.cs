using Assets.Scripts.SuitShop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.ImportData
{
    public class AIList : MonoBehaviour
    {
        public Dictionary<int, AIInfo> AIJumpers;
        public Dictionary<int, List<AIInfo>> JumpersByCountry = new Dictionary<int, List<AIInfo>>();
        [SerializeReference] Suits suits;
        private static AIList instance;
        private CountryList countryList;
        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            countryList = GetComponent<CountryList>();

            instance = this;
            DontDestroyOnLoad(gameObject);

            string fullPath = Path.Combine(Application.streamingAssetsPath, "jumpers.json");
            using (StreamReader r = new StreamReader(fullPath))
            {
                string json = r.ReadToEnd();
                List<AIInfo> list = JsonConvert.DeserializeObject<List<AIInfo>>(json);
                foreach (var item in list)
                {
                    foreach (var i in Enum.GetValues(typeof(SuitEnum)).Cast<SuitEnum>())
                    {
                        item.curSuitID[i] = Mathf.Min(suits.partsDictG()[i].Count() - 1, item.curSuitID[i]);
                        item.curSuitID[i] = Mathf.Max(item.curSuitID[i], 0);
                    }
                    var stats = suits.CalculateStats(item.curSuitID);
                    item.lubricationFactor = stats.Item1;
                    item.suitFactor = stats.Item2;
                    item.bootsFactor = stats.Item3;
                    item.Country = item.Country.ToLower();

                    int countryId = countryList.getCountry(item.Country.ToLower()).id;
                    if (!JumpersByCountry.ContainsKey(countryId))
                        JumpersByCountry.Add(countryId, new List<AIInfo>());
                    JumpersByCountry[countryId].Add(item);
                };
                AIJumpers = list.ToDictionary(a => a.Number);
                foreach(var country in JumpersByCountry.Values)
                {
                    country.Sort((AIInfo a,AIInfo b) => {return (int)Mathf.Sign(a.avgStats() - b.avgStats()); });
                }
            }
        }
    }

    public class AIInfo : Competitor
    {
        [Header("Info")]
        public int Number;
        public string Name;
        public String Country;
        public int Level;

        [Header("InRun")]
        public float controlFactorRun = 1;
        public float swingsFactorRun = 1;
        public float lubricationFactor = 1;

        [Header("Fly")]
        public float controlFactorFly = 1;
        public float swingsFactorFly = 1;
        public float suitFactor = 1;

        [Header("Down")]
        public float controlFactorDown = 1;
        public float swingsFactorDown = 1;
        public float bootsFactor = 1;

        [Header("Suit")]
        public Dictionary<SuitEnum, int> curSuitID;

        public string getCountry()
        {
            return Country;
        }

        public string getName()
        {
            return Name;
        }

        public bool isAI()
        {
            return true;
        }

        public int getAiId()
        {
            return Number;
        }

        public float avgStats()
        {
            return (swingsFactorDown + swingsFactorFly + controlFactorDown + swingsFactorRun + controlFactorFly + controlFactorRun )/6;
        }

        public Competitor GetNextCompetitor()
        {
            return this;
        }

        public override bool Equals(object obj)
        {
            return obj is AIInfo info &&
                   Number == info.Number &&
                   Name == info.Name &&
                   Country == info.Country &&
                   controlFactorRun == info.controlFactorRun &&
                   swingsFactorRun == info.swingsFactorRun &&
                   lubricationFactor == info.lubricationFactor &&
                   controlFactorFly == info.controlFactorFly &&
                   swingsFactorFly == info.swingsFactorFly &&
                   suitFactor == info.suitFactor &&
                   controlFactorDown == info.controlFactorDown &&
                   swingsFactorDown == info.swingsFactorDown &&
                   bootsFactor == info.bootsFactor &&
                   EqualityComparer<Dictionary<SuitEnum, int>>.Default.Equals(curSuitID, info.curSuitID);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Number);
            hash.Add(Name);
            hash.Add(Country);
            hash.Add(controlFactorRun);
            hash.Add(swingsFactorRun);
            hash.Add(lubricationFactor);
            hash.Add(controlFactorFly);
            hash.Add(swingsFactorFly);
            hash.Add(suitFactor);
            hash.Add(controlFactorDown);
            hash.Add(swingsFactorDown);
            hash.Add(bootsFactor);
            return hash.ToHashCode();
        }
    }

    public interface Competitor
    {
        public string getName();
        public string getCountry();
        public bool isAI();
        public int getAiId();
        /// <summary>
        /// In group return current jumper and change pointer to next, in no group return this
        /// </summary>
        /// <returns></returns>
        public Competitor GetNextCompetitor();
    }
}
