using Assets.Scripts.Competition;
using Assets.Scripts.ImportData;
using Assets.Scripts.SuitShop;
using Assets.Scripts.Tournaments;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.ImportData
{
    public class DataHolder : MonoBehaviour
    {
        public Dictionary<SuitEnum, int> playerSuit = new Dictionary<SuitEnum, int>();
        public NormalTournament tournament;
        public bool isTraining = false;
        private static DataHolder instance;
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }
}