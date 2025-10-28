using UnityEngine;

namespace Assets.Scripts.Menu
{

    public class InfoMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject manual, credits, mainMenu;
        private void OnEnable()
        {
            manual.SetActive(true);
            credits.SetActive(false);
        }

        public void Back()
        {
            this.gameObject.SetActive(false);
            mainMenu.SetActive(true);
        }

        public void HideCredits()
        {
            credits.SetActive(false);
            manual.SetActive(true);
        }

        public void ShowCredits()
        {
            credits.SetActive(true);
            manual.SetActive(false);
        }
    }
}