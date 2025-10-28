using Assets.Scripts.ImportData;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


namespace Assets.Scripts.SuitShop
{
    public class Suits : MonoBehaviour
    {
        [SerializeField] private SuitsGlobal global;
        [SerializeField] private SuitPart[] partsToExport;
        [SerializeField] private Equipment equipment;
        private Dictionary<SuitEnum, List<SuitPart>> partsDict;
        public Dictionary<SuitEnum, int> wearID = new Dictionary<SuitEnum, int>();
        private ShopElements shopElements;

        [SerializeField] float minFactor;// = 0.75f;
        [SerializeField] float maxFactor;// = 1.3f;
        [SerializeField] MaterialsContainer materialsContainer;

        DataHolder dataHolder;
        private SuitPart currenSelected;

        public string savePath = "Assets/Models/Suits/Suits.json";
        [ContextMenu("Export to File")]
        public void Export()
        {

            SuitsDTO[] suitsDTO = new SuitsDTO[partsToExport.Length];
            for(int i = 0; i < partsToExport.Length; i++)
            {
                suitsDTO[i] = new SuitsDTO(partsToExport[i]);
            }

            using (StreamWriter w = new StreamWriter(savePath))
            {
                string json = JsonConvert.SerializeObject(suitsDTO, Formatting.Indented);
                w.Write(json);
            }
        }
         

        public void AwakeTick()
        {
            dataHolder = GameObject.Find("DataHolder").GetComponent<DataHolder>();
            shopElements = GetComponent<ShopElements>();
            if (dataHolder.playerSuit.Count() > 0)
            {
                foreach (var (part,id) in dataHolder.playerSuit)
                {
                    wearID.TryAdd(part, id);
                }
            }
            else
            {
                foreach (SuitEnum part in Enum.GetValues(typeof(SuitEnum)))
                {
                    wearID.Add(part, 0);
                    dataHolder.playerSuit.Add(part, 0);
                    partsDictG()[part][wearID[part]].clothe(materialsContainer);
                    partsDictG()[part][wearID[part]].buy = true;
                }
            }
            RefreshStats();
            shopElements.Money.text = string.Format("{0:F2}", equipment.SetCur(wearID));

            shopElements.Aero.minValue = minFactor;
            shopElements.Aero.maxValue = maxFactor;
            shopElements.Lubrication.minValue = minFactor;
            shopElements.Lubrication.maxValue = maxFactor;
            shopElements.Boots.minValue = minFactor;
            shopElements.Boots.maxValue = maxFactor;
        }

        private void OnEnable()
        {
        }

        private void Awake()
        {
            materialsContainer = GetComponent<MaterialsContainer>();
            partsDictG();
        }

        private void OnValidate()
        {
            if (partsToExport != null)
            {
                foreach (var part in partsToExport)
                {
                    if (materialsContainer != null)
                    {
                        Array.Resize(ref part.materialsSettings, materialsContainer.MaterialsDict[part.Type].Length);
                    }
                    if (part.factor > maxFactor)
                        part.factor = maxFactor;
                    else if (part.factor < minFactor)
                        part.factor = minFactor;
                }
            }
        }

        public Dictionary<SuitEnum, List<SuitPart>> partsDictG()
        {
            if (partsDict != null)
                return partsDict;
            partsDict = new Dictionary<SuitEnum, List<SuitPart>>();
            foreach (var part in global.parts)
            {
                if (!partsDict.ContainsKey(part.Type))
                {
                    partsDict.Add(part.Type, new List<SuitPart>());
                }
                partsDict[part.Type].Add(part);
            }
            foreach (List<SuitPart> item in partsDict.Values)
            {
                item.Sort();
            }
            return partsDict;
        }

        public void nextSuit() => next(SuitEnum.Suit);
        public void nextSki() => next(SuitEnum.Ski);
        public void nextHelmet() => next(SuitEnum.Helmet);
        public void nextBoots() => next(SuitEnum.Boots);
        public void nextGloves() => next(SuitEnum.Gloves);
        public void nextGoogle() => next(SuitEnum.Front);

        public void next(SuitEnum type)
        {
            partsDictG();
            wearID[type]++;
            if (wearID[type] >= partsDict[type].Count())
            {
                wearID[type] = 0;
            }
            partsDict[type][wearID[type]].clothe(materialsContainer);
            RefreshStats();
            RefreshInfo(partsDict[type][wearID[type]]);
        }

        public void RefreshStats()
        {
            var res = CalculateStats(wearID);
            shopElements.Boots.value = res.Item3;
            shopElements.Lubrication.value = res.Item1;
            shopElements.Aero.value = res.Item2;
            var own = CalculateStats(dataHolder.playerSuit);
            shopElements.AeroTEXT.text = "AERODYNAMICS "+ (res.Item2 - own.Item2).ToString("0.00");
            shopElements.LubricationTEXT.text = "LUBRICATION " + (res.Item1 - own.Item1).ToString("0.00");
            shopElements.BootsTEXT.text = "BOOTS AND BINDINGS " + (res.Item3 - own.Item3).ToString("0.00");
        }

        /// <summary>
        /// Return in order lubric, aero,boots
        /// </summary>
        public (float, float, float) CalculateStats(Dictionary<SuitEnum, int> wearID)
        {
            partsDictG();
            float boots, lubric, aero;
            boots = partsDict[SuitEnum.Boots][wearID[SuitEnum.Boots]].factor;
            lubric = partsDict[SuitEnum.Ski][wearID[SuitEnum.Ski]].factor;
            aero = partsDict[SuitEnum.Suit][wearID[SuitEnum.Suit]].factor;

            aero *= partsDict[SuitEnum.Helmet][wearID[SuitEnum.Helmet]].factor;
            aero *= partsDict[SuitEnum.Gloves][wearID[SuitEnum.Gloves]].factor;
            aero *= partsDict[SuitEnum.Front][wearID[SuitEnum.Front]].factor;
            lubric *= partsDict[SuitEnum.Helmet][wearID[SuitEnum.Helmet]].factor;
            lubric *= partsDict[SuitEnum.Front][wearID[SuitEnum.Front]].factor;

            return (lubric, aero, boots);
        }

        public void RefreshInfo(SuitPart part)
        {
            shopElements.Name.text = part.name;
            shopElements.Factor.text = string.Format("Factory: {0:F2}", part.factor);
            shopElements.Cost.text = string.Format("Cost: {0:F2}", part.cost);

            if (part.buy)
                shopElements.Buy.text = "Put on";
            else
                shopElements.Buy.text = "Buy";

            if (dataHolder.playerSuit[part.Type] == wearID[part.Type])
                shopElements.Name.fontStyle = FontStyle.Bold;
            else
                shopElements.Name.fontStyle = FontStyle.Normal;

            currenSelected = part;
        }

        public void buy()
        {
            shopElements.Money.text = string.Format("{0:F2}", equipment.Buy(currenSelected));
            RefreshStats();
            RefreshInfo(currenSelected);
        }

        public void selectSuit() => RefreshInfo(partsDictG()[SuitEnum.Suit][wearID[SuitEnum.Suit]]);
        public void selectHelmet() => RefreshInfo(partsDictG()[SuitEnum.Helmet][wearID[SuitEnum.Helmet]]);
        public void selectSki() => RefreshInfo(partsDictG()[SuitEnum.Ski][wearID[SuitEnum.Ski]]);
        public void selectGloves() => RefreshInfo(partsDictG()[SuitEnum.Gloves][wearID[SuitEnum.Gloves]]);
        public void selectGoogle() => RefreshInfo(partsDictG()[SuitEnum.Front][wearID[SuitEnum.Front]]);
        public void selectBoots() => RefreshInfo(partsDictG()[SuitEnum.Boots][wearID[SuitEnum.Boots]]);

        public float getAero() => shopElements.Aero.value;
        public float getLubri() => shopElements.Lubrication.value;
        public float getBoots() => shopElements.Boots.value;

        public void prevSuit() => prev(SuitEnum.Suit);
        public void prevSki() => prev(SuitEnum.Ski);
        public void prevHelmet() => prev(SuitEnum.Helmet);
        public void prevBoots() => prev(SuitEnum.Boots);
        public void prevGloves() => prev(SuitEnum.Gloves);
        public void prevGoogle() => prev(SuitEnum.Front);

        public void prev(SuitEnum type)
        {
            partsDictG();
            wearID[type]--;
            if (wearID[type] < 0)
            {
                wearID[type] = partsDict[type].Count() - 1;
            }
            partsDict[type][wearID[type]].clothe(materialsContainer);
            RefreshStats();
            RefreshInfo(partsDict[type][wearID[type]]);
        }

        public void clothe(Dictionary<SuitEnum, int> curID)
        {
            foreach (var (k, v) in curID)
            {
                partsDictG()[k][v].clothe(materialsContainer);
            }
            wearID = curID.ToDictionary(e => e.Key, e => e.Value);
        }
    }

    [System.Serializable]
    public class SuitPart : IComparable<SuitPart>
    {
        public SuitEnum Type;

        public float cost;
        public float factor;
        public string name;
        public bool buy = false;
        //[SerializeField] protected Material[] materials;
        public SuitMaterialOption[] materialsSettings;

        public SuitPart() { }
        public SuitPart(SuitsDTO dto)
        {
            Type = dto.Type;
            cost = dto.cost;
            factor = dto.factor;
            name = dto.name;
            buy = dto.buy;
            materialsSettings = new SuitMaterialOption[dto.metallics.Length];
            for(int i = 0; i < materialsSettings.Length; i++)
            {
                materialsSettings[i] = new SuitMaterialOption(dto.metallics[i], dto.smooth[i], new Color(dto.colors[i].x,dto.colors[i].y, dto.colors[i].z), dto.textures[i] != null ? Resources.Load<Texture2D>(System.IO.Path.ChangeExtension("Versions/"+dto.textures[i],null)) : null);
            }
        }

        public void clothe(MaterialsContainer materialsContainer)
        {
            for (int i = 0; i < materialsSettings.Length; ++i)
            {
                materialsContainer.MaterialsDict[Type][i].SetFloat("_Metallic", materialsSettings[i].metallic);
                materialsContainer.MaterialsDict[Type][i].SetFloat("_Smoothness", materialsSettings[i].smooth);
                if (materialsSettings[i].texture == null)
                {
                    materialsContainer.MaterialsDict[Type][i].SetTexture("_BaseMap", null);
                    materialsContainer.MaterialsDict[Type][i].color = materialsSettings[i].color;
                }
                else
                {
                    materialsContainer.MaterialsDict[Type][i].color = materialsSettings[i].color;
                    materialsContainer.MaterialsDict[Type][i].SetTexture("_BaseMap", materialsSettings[i].texture);
                }
            }
        }

        public int Compare(SuitPart x, SuitPart y)
        {
            return Math.Sign(x.factor - y.factor);
        }

        public int CompareTo(SuitPart other)
        {
            return Math.Sign(this.factor - other.factor);
        }
    }

    [System.Serializable]
    public class SuitMaterialOption
    {
        [SerializeField] public float metallic;
        [SerializeField] public float smooth;
        [SerializeField] public Color color;
        [SerializeField] public Texture2D texture;

        public SuitMaterialOption(float metallic, float smooth, Color color, Texture2D texture)
        {
            this.metallic = metallic;
            this.smooth = smooth;
            this.color = color;
            this.texture = texture;
        }
        public SuitMaterialOption() { }
    }

}

