using Assets.Scripts.Jumping.StaticInfo;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;


namespace Assets.Scripts.ImportData
{
    public class SkiJumpsImporter : MonoBehaviour
    {
        [Header("InResources")]
        public string from;
        [Header("InStreamingAssets")]
        public string to;

        [ContextMenu("CreateJSON")]
        public void Export()
        {
            SkiJumpInfoGlobal[] skijumps = Resources.LoadAll<SkiJumpInfoGlobal>(from);
            for (int i = 0; i < skijumps.Length; i++)
            {
                string s = Path.Combine(Application.streamingAssetsPath, to, skijumps[i].name + ".json");
                using (StreamWriter w = new StreamWriter(s))
                {
                    string json = JsonConvert.SerializeObject(skijumps[i], Formatting.Indented);
                    w.Write(json);
                }
            }
        }

        private void OnEnable()
        {
            string partPath = Path.Combine(Application.streamingAssetsPath, to);
            string[] files = Directory.GetFiles(partPath, "*.json", SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                string name = Path.ChangeExtension(Path.GetFileName(file), null);
                SkiJumpInfoGlobal info = Resources.Load<SkiJumpInfoGlobal>(Path.Combine(from, name));
                if (info != null && File.Exists(file))
                {
                    using (StreamReader w = new StreamReader(file))
                    {
                        SkiJumpInfoGlobal data = ScriptableObject.CreateInstance<SkiJumpInfoGlobal>();
                        JsonSerializer jsonSerializer = new JsonSerializer();
                        jsonSerializer.Populate(w, data);
                        //JsonUtility.FromJsonOverwrite(w.ReadToEnd(),data);
                        // SkiJumpInfoGlobal data = JsonConvert.DeserializeObject<SkiJumpInfoGlobal>(w.ReadToEnd());
                        info.name = data.name;
                        info.p = data.p;
                        info.record = data.record;
                        info.controlValue = data.controlValue;
                        info.country3Dig = data.country3Dig;
                        info.recordJumper = data.recordJumper;
                        info.areaUp = data.areaUp;
                        info.badHeight = data.badHeight;
                        info.bestUp = data.bestUp;
                        info.changeMeter = data.changeMeter;
                        info.crashHeight = data.crashHeight;
                        info.divisionSwing = data.divisionSwing;
                        info.divisionWindSwingMAX = data.divisionWindSwingMAX;
                        info.divisionWindSwingMIN = data.divisionWindSwingMIN;
                        info.fullName = data.fullName;
                        info.gateDefault = data.gateDefault;
                        info.resistance = data.resistance;
                        info.resistanceDown = data.resistanceDown;
                        info.resistanceHill = data.resistanceHill;
                        info.windFactor = data.windFactor;
                        info.swingFrequencyOther = data.swingFrequencyOther;
                        info.swingFrequencyFly = data.swingFrequencyFly;
                        info.pointMeter = data.pointMeter;
                        info.pointGate = data.pointGate;
                        info.onePoisitionDif = data.onePoisitionDif;
                        info.K = data.K;
                        info.HS = data.HS;
                        info.OkHeight = data.OkHeight;
                        info.NiceHeight = data.NiceHeight;
                        info.lostHeightFactor = data.lostHeightFactor;
                        info.increaseSpeed = data.increaseSpeed;
                        info.maxSpeed = data.maxSpeed;
                        info.gustsForce = data.gustsForce;
                        info.gustsFreq = data.gustsFreq;
                        info.lostSpeedFactorLate = data.lostSpeedFactorLate;
                        info.multiplyResistanceLate = data.multiplyResistanceLate;
                    }
                }
            }
        }
    }
}