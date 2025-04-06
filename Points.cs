using Assets.WebDTO;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows;

public class Points : MonoBehaviour
{
    float[] points= new float[5];
    float windPoint = 0;
    float distPoint = 0;
    float avgPoint = 0;
    [SerializeReference] InRunAnim inRun;
    [SerializeReference] FlyAnim fly;
    [SerializeReference] LandingAnim landing;
    [SerializeReference] Fly landingStyle;
    WindController windCntrl;
    [SerializeField] SkiJumpInfo jumpInfo;
    [SerializeField] UiController UiController;
    WebControler webControler;
    //[SerializeReference] public PointAnimI inRun, fly, landing, landingStyle;

    private void Awake()
    {
        webControler = GetComponent<WebControler>();
        windCntrl = GetComponent<WindController>();
    }
    private void OnEnable()
    {
        avgPoint = (inRun.GetPoint() * 1) + (fly.GetPoint() * 5) + (landing.GetPoint() * 5) + (landingStyle.GetPoint() * 7) ;
        avgPoint += Mathf.Max(0,1.8f + ((landingStyle.getDistance() + jumpInfo.changeMeter) - jumpInfo.K) / (jumpInfo.HS - jumpInfo.K));

        for (int i = 0; i < 5; i++)
        {
            points[i] = Random.Range(-1.01f,1.01f) + avgPoint;
            points[i] -= points[i]%0.5f;
            points[i] = Mathf.Min(points[i], 20);
            points[i] = Mathf.Max(points[i], 0);
        }
        windPoint = windCntrl.GetPoint()*jumpInfo.windFactor;
        distPoint = ((landingStyle.getDistance() + jumpInfo.changeMeter - jumpInfo.K)*jumpInfo.pointMeter)+60;
        avgPoint = points.Sum() - points.Min() - points.Max();

        PointsStat stat = new PointsStat() { 
            dist = landingStyle.getDistance() + jumpInfo.changeMeter,
            pointsJury = points,
            wind = windPoint,
            distPoints = distPoint,
            sum = distPoint + windPoint + points.Sum() - points.Min() - points.Max()
        };
        UiController.ShowStat(stat);
        webControler.sendStat(stat);
    }

    private void OnDisable()
    {
        UiController.hideStat();
        for (int i = 0; i < 5; i++)
        {
            points[i] = 0;
        }
        windPoint = 0;
        distPoint = 0;
        avgPoint = 0;
    }
}
