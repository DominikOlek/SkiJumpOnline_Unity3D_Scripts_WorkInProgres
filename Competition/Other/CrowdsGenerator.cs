using UnityEngine;


namespace Assets.Scripts.Competition.Other
{
    public class CrowdsGenerator : MonoBehaviour
    {
        public float density = 0.5f;
        [SerializeField] GameObject[] prefabs;
        [SerializeField] Transform lookAtObject;
        private MeshFilter meshFilter;
        MeshRenderer meshRenderer;

        public void Generate(AnimVATTexture[] text)
        {
            for (int i = 0; i < prefabs.Length; i++)
            {
                prefabs[i].GetComponent<MeshRenderer>().sharedMaterial = text[i].materialO;
            }

            if (meshFilter == null)
            {
                meshFilter = GetComponent<MeshFilter>();
            }

            if (meshFilter == null || prefabs == null || prefabs.Length == 0)
            {
                Debug.LogError("Not Found MeshFilter or Prefab!");
                return;
            }

            Mesh mesh = meshFilter.sharedMesh;
            Vector3[] vertices = mesh.vertices;

            foreach (Vector3 vertex in vertices)
            {
                if (Random.Range(0f, 1.0f) < density)
                {
                    Vector3 worldPos = meshFilter.transform.TransformPoint(vertex);
                    GameObject instance = Instantiate(prefabs[Random.Range(0, prefabs.Length)], worldPos, Quaternion.identity, transform);
                    Vector3 target = new Vector3(lookAtObject.transform.position.x, instance.transform.position.y, lookAtObject.transform.position.z);
                    instance.transform.LookAt(target);
                    instance.transform.Rotate(0, Random.Range(-15f, 15f), 0);

                    //if (!instance.TryGetComponent<MeshRenderer>(out meshRenderer))
                    //{
                    //    meshRenderer = instance.GetComponentInChildren<MeshRenderer>();
                    //}
                    //MaterialPropertyBlock temp = new MaterialPropertyBlock();
                    //temp.SetFloat("_TexIndex", (int)Random.Range(0, 10));
                    //meshRenderer.SetPropertyBlock(temp);
                }
            }
        }
    }
}
