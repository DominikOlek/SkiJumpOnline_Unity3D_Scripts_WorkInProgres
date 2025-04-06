using UnityEngine;

public class FitToGorund : MonoBehaviour
{
    public void Fit(float speed, RaycastHit hit) {
        Vector3 surfaceNormal = hit.normal;

        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;
        Vector3 v = targetRotation.eulerAngles;
        v.x -= 31.5f;
        v.z = 0;
        v.y = 90;
        targetRotation = Quaternion.Euler(v);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 5f * Time.deltaTime);

        // Ruch do przodu wzd³u¿ powierzchni
        Vector3 slideDirection = Vector3.ProjectOnPlane(transform.forward, surfaceNormal).normalized;
        slideDirection.z = 0;

        transform.position += slideDirection * speed * Time.deltaTime;
    }
}
