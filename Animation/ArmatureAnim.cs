using UnityEngine;

namespace Assets.Scripts.Animation
{
    public class ArmatureAnim : MonoBehaviour
    {
        public Transform bodyMain, back, armLeft, armRight, backLeft, backRight, legLeft, legRight, main;
        //18        1     9         5       4           3           12      15      Bone 
        public AnimationControler animCtrl;
    }
}