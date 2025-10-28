using Assets.Scripts.Jumping.StaticInfo;
using Assets.Scripts;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.SuitShop;

namespace Assets.Scripts.ImportData
{
    public class SingleInfo
    {
        public float controlFactorRun = 0.8f;
        public float swingsFactorRun = 0.8f;

        public float controlFactorFly = 0.8f;
        public float swingsFactorFly = 0.8f;

        public float controlFactorDown = 0.8f;
        public float swingsFactorDown = 0.8f;

        public Dictionary<SuitEnum, int> wearing;


        public string name;
        public string Country3dig;
        public float money = 1.0f;
        public int winLevel = 0;

        public SingleInfo() { }

        public SingleInfo(JumperInfoGlobal jumperInfo, Dictionary<SuitEnum, int> wearing)
        {
            this.controlFactorRun = jumperInfo.controlFactorRun;
            this.swingsFactorRun = jumperInfo.swingsFactorRun;
            this.controlFactorFly = jumperInfo.controlFactorFly;
            this.swingsFactorFly = jumperInfo.swingsFactorFly;
            this.controlFactorDown = jumperInfo.controlFactorDown;
            this.swingsFactorDown = jumperInfo.swingsFactorDown;
            this.wearing = wearing;
            this.name = jumperInfo.Name;
            Country3dig = jumperInfo.Country3dig;
            this.money = jumperInfo.money;
            this.winLevel = jumperInfo.winLevel;
        }
    }
}