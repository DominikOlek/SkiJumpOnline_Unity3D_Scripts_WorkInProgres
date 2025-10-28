using Assets.Scripts.Animation;
using Assets.Scripts.ImportData;
using Assets.Scripts.SuitShop;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Jumping.StaticInfo
{
    [CreateAssetMenu(fileName = "JumperGlobalData", menuName = "Data/Global")]
    public class JumperInfoGlobal : ScriptableObject, Competitor
    {
        public static float MINFACTOR = 0.85f;
        public static float MAXFACTOR = 1.5f;

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


        public JumpState jumpState;
        public Landing landingStyle;

        [Header("Others")]
        public string Name;
        public string Country3dig;
        public float money = 1.0f;
        public int winLevel = 0;


        private const float middleBorder = 1.0f;
        private const float proBorder = 1.25f;

        public string getCountry()
        {
            return Country3dig;
        }

        public string getName()
        {
            return Name;
        }

        public bool isAI()
        {
            return false;
        }

        public int getAiId()
        {
            return 0;
        }
        
        public Competitor GetNextCompetitor()
        {
            return this;
        }

        public void setSwingRun(float val) => swingsFactorRun=Mathf.Clamp(val,0.85f,1.5f);
        public void setSwingFly(float val) => swingsFactorFly = Mathf.Clamp(val, 0.85f, 1.5f);
        public void setSwingDown(float val) => swingsFactorDown = Mathf.Clamp(val, 0.85f, 1.5f);
        public void setCtrlDown(float val) => controlFactorDown = Mathf.Clamp(val, 0.85f, 1.5f);
        public void setCtrlFly(float val) => controlFactorFly = Mathf.Clamp(val, 0.85f, 1.5f);
        public void setCtrlRun(float val) => controlFactorRun = Mathf.Clamp(val, 0.85f, 1.5f);
        /// <summary>
        /// Level [1,2,3]
        /// </summary>
        /// <returns></returns>
        public int getLevel()
        {
            switch(Mathf.Min(MeanDown(),MeanFly(),MeanRun())){
                case float i when i < middleBorder:
                    return 1;
                case float i when i < proBorder:
                    return 2;
                default:
                    return 3;
            }
        }

        public bool isAvailableLevel(int lvl)
        {
            if(getLevel() > lvl) return true;
            if(getLevel() < lvl) return false;
            if(getLevel() == lvl && winLevel + 1 == lvl) return true;
            return false;
        }

        public float MeanDown() => (swingsFactorDown + controlFactorDown) / 2;
        public float MeanFly() => (swingsFactorFly + controlFactorFly) / 2;
        public float MeanRun() => (swingsFactorRun + controlFactorRun) / 2;


        public void SetValue(SingleInfo info)
        {
            name = info.name;
            controlFactorDown = info.controlFactorDown;
            controlFactorFly = info.controlFactorFly;
            controlFactorRun = info.controlFactorRun;
            Country3dig = info.Country3dig;
            money = info.money;
            swingsFactorDown = info.swingsFactorDown;
            swingsFactorFly = info.swingsFactorFly;
            swingsFactorRun = info.swingsFactorRun; 
            winLevel = info.winLevel;
        }

        public override bool Equals(object obj)
        {
            return obj is JumperInfoGlobal global &&
                   base.Equals(obj) &&
                   name == global.name &&
                   hideFlags == global.hideFlags &&
                   controlFactorRun == global.controlFactorRun &&
                   swingsFactorRun == global.swingsFactorRun &&
                   lubricationFactor == global.lubricationFactor &&
                   controlFactorFly == global.controlFactorFly &&
                   swingsFactorFly == global.swingsFactorFly &&
                   suitFactor == global.suitFactor &&
                   controlFactorDown == global.controlFactorDown &&
                   swingsFactorDown == global.swingsFactorDown &&
                   bootsFactor == global.bootsFactor &&
                   jumpState == global.jumpState &&
                   Name == global.Name &&
                   Country3dig == global.Country3dig &&
                   money == global.money;
        }


        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(0);
            hash.Add(Name);
            hash.Add(Country3dig);
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
}