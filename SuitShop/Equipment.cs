using Assets.Scripts.ImportData;
using Assets.Scripts.Jumping.StaticInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Assets.Scripts.SuitShop
{
    public class Equipment : MonoBehaviour
    {
        [SerializeField] Suits suits;
        [SerializeField] GameObject ShopCanvas, SingleMenu;
        [SerializeField] JumperInfoGlobal jumperInfo;
        DataHolder dataHolder;

        private void Awake()
        {
            dataHolder = GameObject.Find("DataHolder").GetComponent<DataHolder>();
            if (SceneManager.GetActiveScene().buildIndex == 0 && dataHolder != null && dataHolder.playerSuit != null)
            {
                Clothe();
            }
        }

        private void Start()
        {
            dataHolder = GameObject.Find("DataHolder").GetComponent<DataHolder>();
        }

        public float SetCur(Dictionary<SuitEnum, int> wearID)
        {
            dataHolder = GameObject.Find("DataHolder").GetComponent<DataHolder>();
            dataHolder.playerSuit = wearID.ToDictionary(e => e.Key, e => e.Value);
            return jumperInfo.money;
        }

        public float Buy(SuitPart part)
        {
            if (!part.buy && part.cost > jumperInfo.money)
                return jumperInfo.money;
            if (!part.buy)
            {
                jumperInfo.money -= part.cost;
                part.buy = true;
            }
            dataHolder.playerSuit[part.Type] = suits.wearID[part.Type];
            ChangeFactors();
            return jumperInfo.money;
        }

        public void Clothe()
        {
            suits.clothe(dataHolder.playerSuit);
        }

        public void ImportClothe(Dictionary<SuitEnum,int> wearingSuit)
        {
            dataHolder.playerSuit.Clear();
            dataHolder.playerSuit = wearingSuit;
            suits.clothe(dataHolder.playerSuit);
            var stats = suits.CalculateStats(dataHolder.playerSuit);
            jumperInfo.bootsFactor = stats.Item3;
            jumperInfo.suitFactor = stats.Item2;
            jumperInfo.lubricationFactor = stats.Item1;
        }

        public void LeaveShop()
        {
            suits.clothe(dataHolder.playerSuit);
            suits.RefreshStats();
            ChangeFactors();
            ShopCanvas.SetActive(false);
            SingleMenu.SetActive(true);
        }

        private void ChangeFactors()
        {
            jumperInfo.bootsFactor = suits.getBoots();
            jumperInfo.suitFactor = suits.getAero();
            jumperInfo.lubricationFactor = suits.getLubri();
        }

        public void ResetOwner()
        {
            var dict = suits.partsDictG();
            foreach (SuitEnum part in Enum.GetValues(typeof(SuitEnum)))
            {
                int i = 0;
                foreach(var el in dict[part])
                {
                    el.buy = i == 0? true : false;
                    i++;
                }
            }
        }

    }
}