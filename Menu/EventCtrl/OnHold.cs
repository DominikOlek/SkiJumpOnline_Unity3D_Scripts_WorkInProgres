using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Menu.EventCtrl
{
    public class OnHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public UnityEvent OnHoldEvent;
        public float holdThreshold = 0.05f;
        private bool isHolding = false;
        private float holdTimer = 0f;

        public void OnPointerDown(PointerEventData eventData)
        {
            isHolding = true;
            holdTimer = 0f;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isHolding = false;
        }

        void Update()
        {
            if (isHolding)
            {
                holdTimer += Time.deltaTime;

                if (holdTimer >= holdThreshold)
                {
                    OnHoldEvent.Invoke();
                }
            }
        }
    }
}