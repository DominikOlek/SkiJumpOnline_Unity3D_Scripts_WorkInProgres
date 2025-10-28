using Assets.Scripts.ImportData;
using Assets.Scripts.Jumping.Controllers;
using Assets.Scripts.Jumping.StaticInfo;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Competition.Other
{
    public class LoadingScreen : MonoBehaviour
    {
        public Slider progressSlider;
        private CountryList countryList;
        private SkiJumpInfoGlobal loadingSkiJump;
        private string windTxt;

        IGetDifficulty difficultyScript;
        Vector2 wind;

        private TextMeshProUGUI nameField, namelongField, sizesField, recordField, windField, gateField;
        private RawImage country;



        public void Awake()
        {
            try
            {
                countryList = GameObject.FindGameObjectWithTag("ImportController").GetComponent<CountryList>();
            }
            catch (System.Exception e) when (e is MissingComponentException)
            {
                Debug.LogError(e.Message);
            }

            nameField = transform.Find("NAME").GetComponent<TextMeshProUGUI>();
            gateField = transform.Find("GATE").GetComponent<TextMeshProUGUI>();
            namelongField = transform.Find("NAME LONG").GetComponent<TextMeshProUGUI>();
            sizesField = transform.Find("SIZE").GetComponent<TextMeshProUGUI>();
            recordField = transform.Find("RECORD").GetComponent<TextMeshProUGUI>();
            windField = transform.Find("WIND").GetComponent<TextMeshProUGUI>();
            country = transform.Find("COUNTRY").GetComponent<RawImage>();
            progressSlider = transform.Find("PROGRESS").GetComponent<Slider>();
            this.gameObject.SetActive(false);

        }

        public void Show(SkiJumpInfoGlobal loadingSkiJump)
        {
            this.gameObject.SetActive(true);
            this.loadingSkiJump = loadingSkiJump;
            SetWind();
            SetGate();
            namelongField.text = loadingSkiJump.fullName;
            nameField.text = loadingSkiJump.name;
            sizesField.text = $"P-{loadingSkiJump.p} \t K-{loadingSkiJump.K} \t HS-{loadingSkiJump.HS}";
            recordField.text = $"RECORD \t {loadingSkiJump.record.ToString("0.00")}-{loadingSkiJump.recordJumper}";
            try
            {
                country.texture = countryList.getFlag(loadingSkiJump.country3Dig.ToLower());
            }
            catch (System.Exception ex) when (ex is KeyNotFoundException || ex is FileNotFoundException)
            {
                Debug.LogError(ex.Message);
            }
        }

        private void SetWind()
        {
            wind = loadingSkiJump.SetWind();
            if (loadingSkiJump.windStartDirect > 45 && loadingSkiJump.windStartDirect < 135)
                windTxt = "BACK";
            else if (loadingSkiJump.windStartDirect > 225 && loadingSkiJump.windStartDirect < 315)
                windTxt = "FRONT";
            else
                windTxt = "SIDE";

            if (loadingSkiJump.rotateForce < 1f)
                windTxt += "-STABLE";
            else
                windTxt += "-UNSTABLE";

            windField.text = "WIND \t" + windTxt;
        }

        private void SetGate()
        {
            GameObject tmp = GameObject.FindGameObjectsWithTag("CompetitionController").FirstOrDefault();
            if (tmp == null)
            {
                Debug.LogError("Not Found CompetitionController");
                return;
            }
            difficultyScript = tmp.GetComponent<IGetDifficulty>();

            int min = loadingSkiJump.minPosition;
            int max = loadingSkiJump.maxPosition;
            int part = Mathf.RoundToInt((max - min) / 3);
            int lvl = difficultyScript.getDifLevel();
            min = Mathf.Clamp(min + part * (3 - lvl), min, max);
            max = Mathf.Clamp(max + part * (1 - lvl), min, max);

            float pos = (part / 6f) * wind.y;
            pos = min + ((max - min) / 2f + pos);
            if (loadingSkiJump.rotateForce >= 1)
                pos += Random.Range(-1.15f, 1.15f);
            else
                pos += Random.Range(-0.3f, 0.3f);

            loadingSkiJump.startPosition = Mathf.Clamp(Mathf.RoundToInt(pos), loadingSkiJump.minPosition, loadingSkiJump.maxPosition);

            gateField.text = "GATE \t " + loadingSkiJump.startPosition;
        }
    }
}