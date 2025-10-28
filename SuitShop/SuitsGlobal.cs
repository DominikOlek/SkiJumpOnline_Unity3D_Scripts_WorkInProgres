using Assets.Scripts.ImportData;
using Assets.Scripts.SuitShop;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;


namespace Assets.Scripts.SuitShop
{
    [CreateAssetMenu(fileName = "SuitsInfo", menuName = "Data/SuitsInfo")]
    public class SuitsGlobal : ScriptableObject
    {
        public SuitPart[] parts;
        [SerializeField] string fileName = "Suits.json";

        private void OnEnable()
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, fileName);
            using (StreamReader r = new StreamReader(fullPath))
            {
                string data = r.ReadToEnd();
                SuitsDTO[] objects = JsonConvert.DeserializeObject<SuitsDTO[]>(data);
                parts = new SuitPart[objects.Length];
                int i = 0;
                foreach (SuitsDTO obj in objects)
                {
                    parts[i] = new SuitPart(obj);
                    i++;
                }
            }
        }

        public void Export()
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, fileName);
            SuitsDTO[] suitsDTO = new SuitsDTO[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                suitsDTO[i] = new SuitsDTO(parts[i]);
            }

            using (StreamWriter w = new StreamWriter(fullPath))
            {
                string json = JsonConvert.SerializeObject(suitsDTO, Formatting.Indented);
                w.Write(json);
            }
        }
    }
}