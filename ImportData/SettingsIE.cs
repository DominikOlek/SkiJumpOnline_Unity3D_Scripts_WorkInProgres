using Assets.Scripts.ImportData;
using Assets.Scripts.SuitShop;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using Assets.Scripts.Jumping.StaticInfo;
using Assets.Scripts;
using UnityEngine.Rendering;


namespace Assets.Scripts.ImportData
{
    public class SettingsIE : MonoBehaviour
    {
        private static SettingsIE instance;
        [SerializeField] private Settings settings;

        void Start()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            //try
            //{
            //    string fullPath = Path.Combine(Application.streamingAssetsPath, "settings.json");
            //    using (StreamReader r = new StreamReader(fullPath))
            //    {
            //        string json = r.ReadToEnd();
            //        Settings settingsData = JsonConvert.DeserializeObject<Settings>(json);
            //        settings.SetData(settingsData);
            //    }
            //}
            //catch (Exception e) {
            //    settings.SetData(new Settings { SoundVolume = 0.99f,MouseXSens=0.5f,MouseYSens=0.5f });
            //}

            //Settings settingsTMP = ScriptableObject.CreateInstance<Settings>();
            settings.MouseXSens = PlayerPrefs.HasKey("mouseX") ? PlayerPrefs.GetFloat("mouseX") : 0.5f;
            settings.MouseYSens = PlayerPrefs.HasKey("mouseY") ? PlayerPrefs.GetFloat("mouseY") : 0.5f;
            settings.SoundVolume = PlayerPrefs.HasKey("volume") ? PlayerPrefs.GetFloat("volume") : 1;
            //settings.SetData(settings);
        }

        public void SaveData()
        {
            //string path = Path.Combine(Application.streamingAssetsPath, "settings") + ".json";
            //using (StreamWriter w = new StreamWriter(path))
            //{
            //    w.Write(JsonConvert.SerializeObject(settings, Formatting.Indented));
            //}


            PlayerPrefs.SetFloat("mouseX", settings.MouseXSens);
            PlayerPrefs.SetFloat("mouseY", settings.MouseYSens);
            PlayerPrefs.SetFloat("volume", settings.SoundVolume);
            PlayerPrefs.Save();
        }
    }
}
