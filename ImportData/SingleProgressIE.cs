using Assets.Scripts;
using Assets.Scripts.Jumping.StaticInfo;
using Assets.Scripts.Menu;
using Assets.Scripts.Menu.EventCtrl;
using Assets.Scripts.SuitShop;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using UnityEngine;


namespace Assets.Scripts.ImportData
{
    public class SingleProgressIE : MonoBehaviour
    {
        public static bool isFirst = true;
        [SerializeField] private int suitsNumber = 10;
        [SerializeField] JumperInfoGlobal jumperInfo;
        [SerializeField] Equipment equipment;
        [SerializeField] SuitsGlobal suitsGlobal;
        ErrorInfo errorInfo;
        private void Awake()
        {
            if (!isFirst)
                return;
            errorInfo = GetComponent<ErrorInfo>();
            isFirst = false;
            SingleMenu menu = GetComponent<SingleMenu>();
            string path = Path.Combine(Application.streamingAssetsPath, "Single") + ".json";
            if (!File.Exists(path))
            {
                menu.Confirm();
                return;
            }
            try
            {
                string data = File.ReadAllText(path);
                SingleInfo singleInfo = JsonConvert.DeserializeObject<SingleInfo>(data);
                foreach (var k in singleInfo.wearing.Keys.ToArray())
                {
                    singleInfo.wearing[k] = Mathf.Clamp(singleInfo.wearing[k], 0, suitsNumber);
                }
                singleInfo.controlFactorFly = Mathf.Clamp(singleInfo.controlFactorFly, JumperInfoGlobal.MINFACTOR, JumperInfoGlobal.MAXFACTOR + 0.2f);
                singleInfo.controlFactorDown = Mathf.Clamp(singleInfo.controlFactorDown, JumperInfoGlobal.MINFACTOR, JumperInfoGlobal.MAXFACTOR + 0.2f);
                singleInfo.controlFactorRun = Mathf.Clamp(singleInfo.controlFactorRun, JumperInfoGlobal.MINFACTOR, JumperInfoGlobal.MAXFACTOR + 0.2f);
                singleInfo.swingsFactorDown = Mathf.Clamp(singleInfo.swingsFactorDown, JumperInfoGlobal.MINFACTOR, JumperInfoGlobal.MAXFACTOR + 0.2f);
                singleInfo.swingsFactorFly = Mathf.Clamp(singleInfo.swingsFactorFly, JumperInfoGlobal.MINFACTOR, JumperInfoGlobal.MAXFACTOR + 0.2f);
                singleInfo.swingsFactorRun = Mathf.Clamp(singleInfo.swingsFactorRun, JumperInfoGlobal.MINFACTOR, JumperInfoGlobal.MAXFACTOR + 0.2f);

                jumperInfo.SetValue(singleInfo);
                equipment.ImportClothe(singleInfo.wearing);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                errorInfo.ShowInfo("Unfortunatelly the progress can not be load, you have to create new character.");
                menu.Confirm();
            }
        }

        public void ExportData()
        {
            try
            {
                DataHolder dataHolder = GameObject.FindGameObjectWithTag("DataHolder").GetComponent<DataHolder>();
                if (dataHolder != null)
                {
                    SingleInfo dto = new SingleInfo(jumperInfo, dataHolder.playerSuit);
                    string path = Path.Combine(Application.streamingAssetsPath, "Single") + ".json";
                    using (StreamWriter w = new StreamWriter(path))
                    {
                        w.Write(JsonConvert.SerializeObject(dto, Formatting.Indented));
                    }
                }
                suitsGlobal.Export();
            }
            catch (Exception)
            {
                errorInfo.ShowInfo("Unfortunatelly the progress can not be save, please try again.");
            }

        }
    }
}