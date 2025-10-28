using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts.Menu
{
    public class ConfirmPopUpScript : MonoBehaviour
    {
        public IConfirmPopUp confirmPopUpScript;
        private Text questionField;
        private Button confirmButton, rejectButton;

        private void Start()
        {
            questionField = transform.Find("TEXT").GetComponent<Text>();
            confirmButton = transform.Find("CONFIRM").GetComponent<Button>();
            rejectButton = transform.Find("REJECT").GetComponent<Button>();
            this.gameObject.SetActive(false);
        }

        public void Show(IConfirmPopUp confirmPopUp)
        {
            confirmPopUpScript = confirmPopUp;
            questionField.text = confirmPopUp.GetConfirmText();
            rejectButton.onClick.AddListener(() => { confirmPopUp.Reject(); this.gameObject.SetActive(false); });
            confirmButton.onClick.AddListener(() => { confirmPopUp.Confirm(); this.gameObject.SetActive(false); });
            this.gameObject.SetActive(true);
        }

    }

    public interface IConfirmPopUp
    {
        public void Confirm();
        public void Reject();
        public string GetConfirmText();
    }
}