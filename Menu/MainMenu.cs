using Assets.Scripts.ImportData;
using Assets.Scripts.Tournaments;
using UnityEngine;

namespace Assets.Scripts.Menu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] GameObject SingleMenu, MultiMenu, Settings, Info;
        [SerializeField] TournamentsController tournaments;
        [SerializeField] GameObject trainingsController;
        private DataHolder holder;

        private void Awake()
        {
            holder = GameObject.Find("DataHolder").GetComponent<DataHolder>();
            if (holder != null) {
                if (holder.tournament != null)
                {
                    tournaments.NextButton(holder.tournament);
                    this.gameObject.SetActive(false);
                }
                else if(holder.isTraining)
                {
                    holder.tournament = null;
                    trainingsController.SetActive(true);
                    this.gameObject.SetActive(false);
                }
                holder.isTraining = false;
            }
        }
        private void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            Cursor.lockState = CursorLockMode.Confined;
        }

        public void SingleButton()
        {
            SingleMenu.SetActive(true);
            this.gameObject.SetActive(false);
        }

        public void InfoButton()
        {
            Info.SetActive(true);
            this.gameObject.SetActive(false);
        }

        public void SettingsButton()
        {
            Settings.SetActive(true);
            this.gameObject.SetActive(false);
        }

        public void Quit() => Application.Quit();

        public void MultiButton()
        {
            MultiMenu.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
}