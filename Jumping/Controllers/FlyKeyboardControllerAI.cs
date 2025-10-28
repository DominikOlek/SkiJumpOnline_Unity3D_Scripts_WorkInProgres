using Assets.Scripts.Competition.Other;
using Assets.Scripts.Jumping.StaticInfo;
using System.Collections;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

namespace Assets.Scripts.Jumping.Controllers
{
    public class FlyKeyboardControllerAI : MonoBehaviour, IFlyKeyboardController
    {
        private bool isFirstStart = true;
        private bool start = false;

        [SerializeField] CinemachineSplineCart jumper;
        SceneObjects sceneObjects;
        SkiJumpInfoGlobal jumpInfo;
        [SerializeField] JumperInfoGlobal jumperInfo;

        private void Start()
        {
            GameObject tmp = GameObject.FindGameObjectsWithTag("SceneData").FirstOrDefault();
            if (tmp == null)
            {
                Debug.LogError("Not Found SceneData");
                return;
            }
            sceneObjects = tmp.GetComponent<SceneObjects>();
            jumpInfo = sceneObjects.skijumpInfo;
            jumper = GetComponent<CinemachineSplineCart>();
        }
        private void OnDisable()
        {
            isFirstStart = true;
            start = false;
        }

        public bool isStart()
        {
            if (isFirstStart)
            {
                isFirstStart = false;
                StartCoroutine(StartRun());
            }
            return start;
        }

        public void resetState()
        {
            isFirstStart = true;
            start = false;
        }

        public bool isJump()
        {
            if (jumper.SplinePosition >= jumper.Spline.CalculateLength() * 0.985f)
            {
                return true;
            }
            float miss = 8.25f - jumperInfo.controlFactorRun * 5.4f; //1.25*8 = 10, 0,75-3.25
            float dif = Mathf.Abs(jumper.SplinePosition - jumpInfo.bestUp);
            if (dif < miss)
            {
                if (Random.Range(0f, 1f) < jumperInfo.controlFactorRun / (1.75f + Mathf.Pow(dif, 4)))
                { //paraboliczny* spadek 
                    return true;
                }
            }
            else if (jumper.SplinePosition > jumpInfo.bestUp)
            {
                return true;
            }
            return false;
        }

        public (bool, bool) isLanding()
        {
            Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, jumpInfo.NiceHeight * 1.125f, LayerMask.GetMask("Hill"));
            if (hit.distance > 0 && hit.transform.tag == "NoLandAI")
            {
                return (false, false);
            }
            float dist = hit.distance;
            float dif = dist - jumpInfo.NiceHeight;
            if (dif < 0f)
                dif = dif / (2.5f * jumperInfo.controlFactorDown);
            else
                dif = -dif / (7f * jumperInfo.controlFactorDown);
            if (Random.Range(0f, 1f) < jumperInfo.controlFactorDown / 2.2f + dif)
            {
                return (true, Random.Range(0f, 1f) < 0.4f * Mathf.Pow(jumperInfo.controlFactorDown, 2)); //0.28 - 0.9
            }
            return (false, false);
        }

        IEnumerator StartRun()
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 3f));
            start = true;
        }
    }
}