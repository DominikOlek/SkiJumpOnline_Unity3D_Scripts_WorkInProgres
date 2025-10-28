using UnityEngine;

namespace Assets.Scripts.etc
{

    public class BonesReimport : MonoBehaviour
    {
#if UNITY_EDITOR
        public Transform[] bones;
        public SkinnedMeshRenderer a;
        [ContextMenu("Get Bones")]
        public void Names()
        {
            foreach (var b in a.bones)
            {
                Debug.Log(b);
            }
        }
        [ContextMenu("Remap Bones")]
        public void Set()
        {
            a.bones = bones;
        }
#endif

    }
}
