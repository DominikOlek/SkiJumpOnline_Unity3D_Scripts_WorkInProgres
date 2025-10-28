using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.SuitShop
{
    [System.Serializable]
    public class SuitMaterialEntry
    {
        public SuitEnum suitPart;
        public Material[] materials;
    }


    public class MaterialsContainer : MonoBehaviour
    {
        public List<SuitMaterialEntry> materials = new List<SuitMaterialEntry>();

        private Dictionary<SuitEnum, Material[]> _materialsDict;

        public Dictionary<SuitEnum, Material[]> MaterialsDict
        {
            get
            {
                if (_materialsDict == null)
                {
                    _materialsDict = new Dictionary<SuitEnum, Material[]>();
                    foreach (var entry in materials)
                    {
                        _materialsDict[entry.suitPart] = entry.materials;
                    }
                }
                return _materialsDict;
            }
        }
    }

    public enum SuitEnum
    {
        Helmet, Suit, Gloves, Boots, Ski, Front
    }
}