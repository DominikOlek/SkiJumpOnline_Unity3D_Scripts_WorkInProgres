using System;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts.Menu.EventCtrl
{
    public class SliderValueWrite : MonoBehaviour
    {
        [SerializeField] string childName = "Value";

        public void Write(Slider slider)
        {
            try
            {
                transform.Find(childName).GetComponent<Text>().text = (slider.value * 100).ToString("0.") + "%";
            }
            catch (MissingComponentException)
            {
                return;
            }
        }

    }
}