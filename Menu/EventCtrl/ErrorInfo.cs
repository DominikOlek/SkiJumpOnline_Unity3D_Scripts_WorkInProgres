using UnityEngine;

namespace Assets.Scripts.Menu.EventCtrl
{
    public class ErrorInfo : MonoBehaviour
    {

        public void ShowInfo(string text)
        {
            GameObject tmp;
            if (tmp = GameObject.FindGameObjectWithTag("UIPopUp"))
            {
                try
                {
                    tmp.GetComponent<InfoPopUp>().Show(text);
                }
                catch (MissingComponentException)
                {
                    return;
                }
            }
        }

    }
}