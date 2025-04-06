using Assets.WebDTO;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UiController : MonoBehaviour
{
    public GameObject overlay,results,oneResult;
    public TextMeshProUGUI[] Judges = new TextMeshProUGUI[5];
    public TextMeshProUGUI Sum,Distance,Wind;
    [SerializeField] float difference = 67f;
    bool minn = false, maxx =false;
    public void ShowStat(float[] pointsJury, float wind,float dist,float distPoints,float sumP) {
        overlay.SetActive(true);
        for (int i = 0; i < 5; i++)
        {
            Judges[i].text = pointsJury[i].ToString();
            if ((pointsJury[i] == pointsJury.Min() && !minn))
            {
                minn = true;
                Judges[i].color = Color.gray;
            }
            if (pointsJury[i] == pointsJury.Max() && !maxx)
            {
                maxx = true;
                Judges[i].color = Color.gray;
            }

        }
        Wind.text = wind.ToString("0.00");
        Distance.text = dist.ToString() +" m";
        Sum.text = sumP.ToString("0.00");
    }

    public void ShowStat(PointsStat stat)
    {
        ShowStat(stat.pointsJury,stat.wind,stat.dist,stat.distPoints,stat.sum);
    }

    public void ShowResults(Dictionary<string, float> res)
    {
        int i = 0;
        foreach (var item in res)
        {
            GameObject newO = Instantiate(oneResult, oneResult.transform.position, oneResult.transform.rotation);
            newO.transform.SetParent(results.transform);
            newO.SetActive(true);
            Vector3 pos = new Vector3(0, 0 - (i * difference), 0);
            newO.transform.localPosition = pos;
            newO.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.Key;
            newO.transform.Find("Points").GetComponent<TextMeshProUGUI>().text = item.Value.ToString("0.00");
            Debug.Log("Gracz "+item.Key+ " wynik: "+item.Value);
            i++;
        }
        results.SetActive(true);
    }

    public void hideStat() {
        overlay.SetActive(false);
        minn = false;
        maxx = false;
        foreach (TextMeshProUGUI item in Judges)
        {
            item.color = Color.black;
        }
    }
}
