using Assets.Scripts.Jumping.Controllers;
using Assets.Scripts.Jumping.StaticInfo;
using System.Collections;
using System.Linq;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

namespace Assets.Scripts.Competition.Other
{
    public class ToBeatLine : MonoBehaviour
    {
        private CinemachineSplineCart line;
        [HideInInspector] IToBeat Controler;
        [SerializeField] SceneObjects sceneObjects;
        [SerializeField] TextMeshProUGUI overlayText;
        SkiJumpInfoGlobal jumpInfo;
        void Start()
        {
            line = GetComponent<CinemachineSplineCart>();
            line.SplinePosition = 0.1f;
            GameObject tmp = GameObject.FindGameObjectsWithTag("CompetitionController").FirstOrDefault();
            if (tmp == null)
            {
                Debug.LogWarning("Not Found CompetitionController");
                return;
            }
            Controler = tmp.GetComponent<IToBeat>();
            jumpInfo = sceneObjects.skiJumpInfoHolder.skiJumpInfo;
            StartCoroutine(UpdateLine());
        }

        protected IEnumerator UpdateLine()
        {
            yield return new WaitForSeconds(1f);
            var data = Controler.DiffToBeat();
            float diff = data.Item1;
            bool isDistance = data.Item2;
            if (!isDistance)
            {
                if (diff > 0)
                {
                    float avgStyle = Controler.getDifLevel() == 3 ? 18 : Controler.getDifLevel() == 2 ? 17 : 15.5f; 
                    line.enabled = true;
                    diff = diff - 3 * avgStyle - sceneObjects.windController.GetCurPoint() * jumpInfo.windFactor;
                    diff = jumpInfo.K + ((diff - 60) / jumpInfo.pointMeter);
                    line.SplinePosition = diff;
                    overlayText.text = diff.ToString("0.00")+" m";
                    overlayText.gameObject.transform.parent.gameObject.SetActive(true);
                }
                else
                {
                    line.enabled = false;
                    line.SplinePosition = 0.1f;
                    overlayText.text = "0 m";
                    overlayText.gameObject.transform.parent.gameObject.SetActive(false);
                }
            }
            else
            {
                line.enabled = true;
                line.SplinePosition = diff;
                overlayText.text = diff.ToString("0.00") + " m";
                overlayText.gameObject.transform.parent.gameObject.SetActive(true);
            }
            StartCoroutine(UpdateLine());
        }

    }
}