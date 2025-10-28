using Assets.Scripts.Competition.Controllers;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Competition
{

    [CreateAssetMenu(fileName = "CompetitionSettings", menuName = "Data/CompetitionSettings")]
    public class CompetitionSettings : ScriptableObject
    {
        public static Dictionary<int,string> ROUNDNAME = new Dictionary<int, string>() { 
            {0,"Qualification"},
            { 1,"First Round"},
            {2,"Second Round" },
            {3,"Third Round" },
            {4,"Fourth Round" }
        };

        public string shortInfo;
        public RoundSettings[] roundSetting;
        public bool qualification = true;
        public int groupSize;
        public GameObject competitionControlerObject;
        [HideInInspector] public GameObject instance;
        [SerializeReference] public ICompetitionControler competitionControler;

        public void EnableCompetition()
        {
            if(instance == null)
                instance = Instantiate(competitionControlerObject);

            competitionControler = instance.GetComponent<ICompetitionControler>();
            competitionControler.SetSetting(this);
        }

        public void DisableCompetition()
        {
            Destroy(instance);
            instance = null;
        }
    }

    [Serializable]
    public class RoundSettings
    {
        public int competitorsInRound;
        public bool isKO = false;
    }
}