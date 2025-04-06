using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WindController : MonoBehaviour, PointAnimI
{
    [SerializeField] SkiJumpInfo jumpInfo;

    private float windSpeed = 0;
    private Vector2 wind;
    private float windDirection = 0;

    float lastDir = 0;
    float lastSpeed = 0;

    [SerializeField] Image arrow;

    float avgWind = 0;
    float frame = 0;
    bool isFly = false;

    float minRot, minForce;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        minRot = jumpInfo.rotateForce / 3;
        minForce = jumpInfo.gustsForce / 2;
        windDirection = jumpInfo.windStartDirect;
        windSpeed = jumpInfo.windStartForce;
        wind = new Vector2(windSpeed * Mathf.Cos(Mathf.Deg2Rad * windDirection), windSpeed * Mathf.Sin(Mathf.Deg2Rad * windDirection));
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
            windSpeed=Mathf.Clamp(windSpeed, 0.3f, 3f);
            if (isFly)
            {
                avgWind += wind.y;
                frame += 1;
            }
        }

        wind = Vector2.Lerp(wind, new Vector2(windSpeed * Mathf.Cos(Mathf.Deg2Rad * windDirection), windSpeed * Mathf.Sin(Mathf.Deg2Rad * windDirection)),Time.deltaTime*5);
        arrow.transform.rotation = Quaternion.Slerp(arrow.transform.rotation, Quaternion.Euler(0, 0, 180 + windDirection), Time.deltaTime * 5);
        arrow.rectTransform.sizeDelta = Vector2.Lerp(arrow.rectTransform.sizeDelta, new Vector2(windSpeed * 40, 50),Time.deltaTime*5);
    }

    public float getSpeed() {
        return wind.y;
    }

    public float getDir()
    {
        return wind.x;
    }

    public void managePointCnt(bool count)
    {
        isFly = count;
    }

    public float GetPoint()
    {
        float res = avgWind / frame;
        avgWind = 0;
        frame = 0;
        return res;
    }
}
