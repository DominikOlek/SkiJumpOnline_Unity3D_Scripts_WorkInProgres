using Assets.Scripts;
using Assets.Scripts.Jumping.StaticInfo;
using Assets.Scripts.SuitShop;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Assets.Scripts.ImportData
{
    public class SuitImporter : MonoBehaviour
    {
        [SerializeField] Suits suits;
        DataHolder dataHolder;
        [SerializeField] JumperInfoGlobal jumperInfo;
        private void Start()
        {
            dataHolder = GameObject.Find("DataHolder").GetComponent<DataHolder>();
            if (dataHolder.playerSuit.Count() > 0)
            {
                suits.clothe(dataHolder.playerSuit);
                var factor = suits.CalculateStats(dataHolder.playerSuit);

                jumperInfo.lubricationFactor = factor.Item1;
                jumperInfo.suitFactor = factor.Item2;
                jumperInfo.bootsFactor = factor.Item3;
            }
            else
            {
                foreach (SuitEnum part in Enum.GetValues(typeof(SuitEnum)))
                {
                    suits.partsDictG();
                    suits.wearID.Add(part, 0);
                    dataHolder.playerSuit.Add(part, 0);
                }
            }
        }
    }
}