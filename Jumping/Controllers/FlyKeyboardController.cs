using UnityEngine;

namespace Assets.Scripts.Jumping.Controllers
{
    public class FlyKeyboardController : MonoBehaviour, IFlyKeyboardController
    {
        public bool isStart()
        {
            return Input.GetKeyDown(KeyCode.Space);
        }

        public bool isJump()
        {
            return Input.GetKeyDown(KeyCode.Space);
        }

        public (bool, bool) isLanding()
        {
            return (Input.GetKey(KeyCode.Mouse0), Input.GetKey(KeyCode.Mouse1));
        }

        public void resetState()
        {
            return;
        }
    }
}
