using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts.Menu
{

    public class InfoPopUp : MonoBehaviour
    {
        public GameObject infoPrefab;

        public void Show(string text)
        {
            GameObject popup = GameObject.Instantiate(infoPrefab);
            popup.transform.SetParent(transform, false);
            popup.transform.localScale = Vector3.one;
            popup.transform.Find("TEXT").GetComponent<Text>().text = text;
            popup.transform.GetComponentInChildren<Button>().onClick.AddListener(() => { Destroy(popup); });
        }
    }
}
