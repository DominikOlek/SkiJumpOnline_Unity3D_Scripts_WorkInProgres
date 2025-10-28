using Assets.Scripts.Jumping.StaticInfo;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts.Menu
{

    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] private Slider mouseX, mouseY, volume;
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private Settings settings;

        public void Hide()
        {
            mainMenu.SetActive(true);
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            mouseX.value = settings.MouseXSens;
            mouseY.value = settings.MouseYSens;
            volume.value = settings.SoundVolume;
        }
    }
}