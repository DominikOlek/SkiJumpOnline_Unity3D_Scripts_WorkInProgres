using Assets.Scripts.Competition;
using Assets.Scripts.Competition.Other;
using Assets.Scripts.Jumping.StaticInfo;
using Assets.Scripts.Jumping.UI;
using Assets.Scripts.WebDTO;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows;


namespace Assets.Scripts.Jumping.Controllers
{
    public class Points : MonoBehaviour
    {
        float[] points = new float[5];
        float windPoint = 0;
        float distPoint = 0;
        float avgPoint = 0;
        ACInRunAnim inRun;
        ACFlyAnim fly;
        ACLandingAnim landing;
        Fly landingStyle;
        public GameObject jumper;
        GameObject jumperChild;
        WindController windCntrl;
        SceneObjects sceneObjects;
        SkiJumpInfoGlobal jumpInfo;
        [SerializeField] UiJumpResult UiController;
        [HideInInspector] ISetResult competitionControler;
        WebControler webControler;
        //[SerializeReference] public PointAnimI inRun, fly, landing, landingStyle;

        private void Awake()
        {
            GameObject tmp = GameObject.FindGameObjectsWithTag("SceneData").FirstOrDefault();
            if (tmp == null)
            {
                Debug.LogError("Not Found SceneData");
                return;
            }
            sceneObjects = tmp.GetComponent<SceneObjects>();
            jumpInfo = sceneObjects.skijumpInfo;
            webControler = GetComponent<WebControler>();
            windCntrl = GetComponent<WindController>();
            tmp = GameObject.FindGameObjectsWithTag("CompetitionController").FirstOrDefault();
            if (tmp == null)
            {
                Debug.LogError("Not Found CompetitionController");
                return;
            }
            competitionControler = tmp.GetComponent<ISetResult>();
        }

        private void Start()
        {
        }

        private void OnEnable()
        {
            jumperChild = jumper.transform.GetChild(0).gameObject;
            inRun = jumperChild.GetComponent<ACInRunAnim>();
            fly = jumperChild.GetComponent<ACFlyAnim>();
            landing = jumperChild.GetComponent<ACLandingAnim>();
            landingStyle = jumper.GetComponent<Fly>();

            avgPoint = (inRun.GetPoint() * 1) + (fly.GetPoint() * 5) + (landing.GetPoint() * 5) + (landingStyle.GetPoint() * 7);
            avgPoint += Mathf.Max(0, 1.8f + ((landingStyle.getDistance() + jumpInfo.changeMeter) - jumpInfo.K) / (jumpInfo.HS - jumpInfo.K));

            for (int i = 0; i < 5; i++)
            {
                points[i] = Random.Range(-1.01f, 1.01f) + avgPoint;
                points[i] -= points[i] % 0.5f;
                points[i] = Mathf.Clamp(points[i],0,20);
            }
            float ww = windCntrl.GetPoint();
            windPoint = ww * jumpInfo.windFactor;
            distPoint = ((landingStyle.getDistance() + jumpInfo.changeMeter - jumpInfo.K) * jumpInfo.pointMeter) + 60;
            avgPoint = points.Sum() - points.Min() - points.Max();

            PointsStat stat = new PointsStat()
            {
                dist = landingStyle.getDistance() + jumpInfo.changeMeter,
                pointsJury = points,
                wind = windPoint,
                distPoints = distPoint,
                sum = distPoint + windPoint + points.Sum() - points.Min() - points.Max(),

                //windX = windCntrl.GetPointTMP(),
                windY = ww
            };
            competitionControler.SetResult(stat);
            if (webControler.isActiveAndEnabled)
                webControler.sendStat(stat);
        }

        private void OnDisable()
        {
            if(UiController)
                UiController.HideStat();
            for (int i = 0; i < 5; i++)
            {
                points[i] = 0;
            }
            windPoint = 0;
            distPoint = 0;
            avgPoint = 0;
        }
    }
}