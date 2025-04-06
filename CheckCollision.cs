using UnityEngine;

public class CheckCollision : MonoBehaviour
{
    public bool trigger = false;

    private void OnTriggerEnter(Collider other)
    {
        trigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        trigger = false;
    }
}
