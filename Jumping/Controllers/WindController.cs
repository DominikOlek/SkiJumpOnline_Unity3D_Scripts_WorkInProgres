using Assets.Scripts.Competition.Other;
using Assets.Scripts.Jumping.StaticInfo;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Jumping.Controllers
{
    public class WindController : MonoBehaviour, PointAnimI
    {
        SceneObjects sceneObjects;
        SkiJumpInfoGlobal jumpInfo;
        CameraOptions cameraPOV;

        private float windSpeed = 0;
        private Vector2 wind;
        private float windDirection = 0;

        float lastDir = 0;
        float lastSpeed = 0;

        [SerializeField] Image arrow;

        float avgWind = 0;
        float frame = 0;
        bool isFly = false;
        float avgWindAll= 0;
        float frameAll = 0;

        float minRot, minForce;

        /// <param name="info"></param>
        /// <returns>Vector2 like wind in game</returns>
        public static Vector2 CalculateWind(SkiJumpInfoGlobal info)
        {
            return new Vector2(info.windStartForce * Mathf.Cos(Mathf.Deg2Rad * info.windStartDirect), info.windStartForce * Mathf.Sin(Mathf.Deg2Rad * info.windStartDirect));
        }

        void Awake()
        {
            GameObject tmp = GameObject.FindGameObjectsWithTag("SceneData").FirstOrDefault();
            if (tmp == null)
            {
                Debug.LogError("Not Found SceneData");
                return;
            }
            sceneObjects = tmp.GetComponent<SceneObjects>();
            jumpInfo = sceneObjects.skijumpInfo;

            //jumpInfo.gustsFreq = Random.Range(0.001f, 0.1f);
            //jumpInfo.gustsForce = Random.Range(0.05f, 0.55f);
            //jumpInfo.rotateForce = Random.Range(0.1f, 2f);
            //jumpInfo.windStartDirect = Random.Range(0f, 359f);
            //jumpInfo.windStartForce = Random.Range(0.5f, 1.5f);

            //jumpInfo.gustsFreq = 0;
            //jumpInfo.gustsForce = 0;
            //jumpInfo.rotateForce = 0;
            //jumpInfo.windStartDirect = 0;
            //jumpInfo.windStartForce = 0;

            minRot = jumpInfo.rotateForce / 3;
            minForce = jumpInfo.gustsForce / 2;
            windDirection = jumpInfo.windStartDirect;
            windSpeed = jumpInfo.windStartForce;
            wind = new Vector2(windSpeed * Mathf.Cos(Mathf.Deg2Rad * windDirection), windSpeed * Mathf.Sin(Mathf.Deg2Rad * windDirection));
            cameraPOV = this.gameObject.GetComponent<CameraCtrl>().CamPov.GetComponent<CameraOptions>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Random.Range(0, 5) < jumpInfo.gustsFreq)
            {
                lastDir = lastDir < 0 ? Random.Range(-jumpInfo.rotateForce, minRot) : lastDir > 0 ? Random.Range(-minRot, jumpInfo.rotateForce) : Random.Range(-jumpInfo.rotateForce, jumpInfo.rotateForce);
                lastSpeed = lastSpeed < 0 ? Random.Range(-minForce, jumpInfo.gustsForce) : lastSpeed > 0 ? Random.Range(-jumpInfo.gustsForce, minForce) : Random.Range(-jumpInfo.gustsForce, jumpInfo.gustsForce);
                windDirection += lastDir;
                windDirection = windDirection >= 360 ? windDirection - 360 : windDirection;
                windDirection = windDirection < 0 ? windDirection + 360 : windDirection;
                windSpeed += lastSpeed;
                windSpeed = Mathf.Clamp(windSpeed, 0.3f, 3f);
            }
            if (isFly)
            {
                avgWind += wind.y;
                //avgWindTMP += wind.x;
                frame += 1;
                //frameTMP += 1;
            } 
            avgWindAll += wind.y;
            frameAll += 1;

            wind = Vector2.Lerp(wind, new Vector2(windSpeed * Mathf.Cos(Mathf.Deg2Rad * windDirection), windSpeed * Mathf.Sin(Mathf.Deg2Rad * windDirection)), Time.deltaTime * 5);
            arrow.transform.rotation = Quaternion.Slerp(arrow.transform.rotation, Quaternion.Euler(0, 0, 180 + windDirection + cameraPOV.GetWindArrowChange()), Time.deltaTime * 5);
            arrow.rectTransform.sizeDelta = Vector2.Lerp(arrow.rectTransform.sizeDelta, new Vector2(windSpeed * 40, 50), Time.deltaTime * 5);
        }

        /// <summary>
        /// </summary>
        /// <returns>return y axis of wind, -3 - 3</returns>
        public float getSpeed()
        {
            return wind.y;
        }
        
        /// <summary>
        /// return X axis of wind
        /// </summary>
        /// <returns></returns>
        public float getDir()
        {
            return wind.x;
        }

        public void managePointCnt(bool count)
        {
            isFly = count;
        }

        /// <summary>
        /// Give windCompensate with restart it, when fly
        /// </summary>
        public float GetPoint()
        {
            if (frame == 0)
                return 0;
            float res = avgWind / frame;
            avgWind = 0;
            frame = 0;
            avgWindAll = 0;
            frameAll = 0;
            return res;
        }

        /// <summary>
        /// Give windCompensate without restart it, in all time
        /// </summary>
        /// <returns></returns>
        public float GetCurPoint()
        {
            if (frameAll == 0)
                return 0;
            return avgWindAll / frameAll;
        }

        //public float GetPointTMP()
        //{
        //    if (frameTMP == 0)
        //        return 0;
        //    float res = avgWindTMP / frameTMP;
        //    avgWindTMP = 0;
        //    frameTMP = 0;
        //    return res;
        //}
    }
}