using Assets.Scripts.Competition;
using Assets.Scripts.Competition.Controllers;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Jumping.UI
{
    public class SkipsController : MonoBehaviour
    {
        private void Start()
        {
            GameObject tmp = GameObject.FindGameObjectsWithTag("CompetitionController").FirstOrDefault();
            ICompetitionControler controler = tmp.GetComponent<ICompetitionControler>();

            int c = this.transform.childCount;
            for(int i = 0; i < c; i++)
            {
                tmp = this.transform.GetChild(i).gameObject;

                int val;
                if (int.TryParse(tmp.name, out val))
                {
                    tmp.GetComponent<Button>().onClick.AddListener(() => {
                        controler.SkipAI(val);
                    });
                }
                else
                {
                    tmp.SetActive(false);
                }
            }
        }
    }
}