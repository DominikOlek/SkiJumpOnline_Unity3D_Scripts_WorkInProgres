using Assets.Scripts.SuitShop;
using Newtonsoft.Json;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;


namespace Assets.Scripts.ImportData
{
    public class SuitsDTO
    {
        public SuitEnum Type;

        public float cost;
        public float factor;
        public string name;
        public bool buy = false;

        public float[] metallics;
        public float[] smooth;
        public SerializableVector3[] colors;
        public string[] textures;

        public SuitsDTO() { }
        public SuitsDTO(SuitPart part)
        {
            Type = part.Type;
            cost = part.cost;
            factor = part.factor;
            name = part.name;
            buy = part.buy;
            metallics = new float[part.materialsSettings.Length];
            smooth = new float[part.materialsSettings.Length];
            colors = new SerializableVector3[part.materialsSettings.Length];
            textures = new string[part.materialsSettings.Length];
            int i = 0;
            foreach (var material in part.materialsSettings)
            {
                metallics[i] = material.metallic;
                smooth[i] = material.smooth;
                colors[i] = new SerializableVector3(new Vector3(material.color.r, material.color.g, material.color.b));
                if (material.texture != null)
                {
                    //string tmp = AssetDatabase.GetAssetPath(material.texture);
                    //int index = tmp.IndexOf("Resources/");
                    //if (index == -1)
                    //    throw new System.Exception(tmp+" Not in Resources index: "+i);
                    //index += "Resources/".Length; 
                    //tmp = tmp.Substring(index);
                    //System.IO.Path.ChangeExtension(tmp,null);
                    textures[i] = material.texture.name;

                }
                i++;
            }
        }

        public Texture2D GetTexture(int index)
        {
            if (textures[index] == null)
                return null;
            return Resources.Load<Texture2D>(textures[index]);
        }
    }

    [System.Serializable]
    public class SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        [JsonIgnore]
        public Vector3 UnityVector
        {
            get
            {
                return new Vector3(x, y, z);
            }
        }

        public SerializableVector3(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public static List<SerializableVector3> GetSerializableList(List<Vector3> vList)
        {
            List<SerializableVector3> list = new List<SerializableVector3>(vList.Count);
            for (int i = 0; i < vList.Count; i++)
            {
                list.Add(new SerializableVector3(vList[i]));
            }
            return list;
        }

        public static List<Vector3> GetSerializableList(List<SerializableVector3> vList)
        {
            List<Vector3> list = new List<Vector3>(vList.Count);
            for (int i = 0; i < vList.Count; i++)
            {
                list.Add(vList[i].UnityVector);
            }
            return list;
        }
    }
}