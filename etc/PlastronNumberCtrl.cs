using Assets.Scripts.ImportData;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


namespace Assets.Scripts.etc
{
    public class PlastronNumberCtrl : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI numberTxt;
        [SerializeField] Material plastron1, plastron2;
        [SerializeField] Texture2D plastron_tex, leaderPlastron_tex;
        private static PlastronNumberCtrl instance;
        private void Awake()
        {
            numberTxt.text = "50";
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void ChangeNumber(int number, bool isLeader = false)
        {
            if (isLeader)
            {
                plastron1.mainTexture = leaderPlastron_tex;
                plastron2.mainTexture = leaderPlastron_tex;
            }
            else
            {
                plastron1.mainTexture = plastron_tex;
                plastron2.mainTexture = plastron_tex;
            }
            numberTxt.text = number.ToString();
        }
    }
}