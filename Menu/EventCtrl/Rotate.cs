using UnityEngine;

namespace Assets.Scripts.Menu.EventCtrl
{
    public class Rotate : MonoBehaviour
    {
        public void forward() => this.transform.Rotate(0, 0, 1);
        public void back() => this.transform.Rotate(0, 0, -1);
    }
}
