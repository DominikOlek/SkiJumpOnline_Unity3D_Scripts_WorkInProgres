using UnityEngine;


namespace Assets.Scripts.Jumping.Controllers
{
    public class CameraCtrl : MonoBehaviour
    {
        public GameObject CamPov, CamTV1, CamTV2;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            RunCamPov();
        }

        public void RunCamTV1()
        {
            CamPov.SetActive(false);
            CamTV1.SetActive(true);
            CamTV2.SetActive(false);
        }

        public void RunCamTV2()
        {
            CamPov.SetActive(false);
            CamTV1.SetActive(false);
            CamTV2.SetActive(true);
        }

        public void RunCamPov()
        {
            CamPov.SetActive(true);
            CamTV1.SetActive(false);
            CamTV2.SetActive(false);
        }
    }
}