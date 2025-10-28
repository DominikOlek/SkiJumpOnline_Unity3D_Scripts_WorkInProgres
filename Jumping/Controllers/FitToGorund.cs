using UnityEngine;

namespace Assets.Scripts.Jumping.Controllers
{
    public class FitToGorund : MonoBehaviour
    {
        public void Fit(float speed, RaycastHit hit)
        {
            Vector3 surfaceNormal = hit.normal;

            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;
            Vector3 v = targetRotation.eulerAngles;
            v.x -= 31.5f;
            v.z = 0;
            v.y = 90;
            targetRotation = Quaternion.Euler(v);

            // Ruch do przodu wzd³u¿ powierzchni
            Vector3 slideDirection = Vector3.ProjectOnPlane(transform.forward, surfaceNormal).normalized;
            //slideDirection.z = 0;

            if (hit.distance < 0.53f)
            {
                slideDirection.y += 0.02f;
            }
            else
            {
                slideDirection.y -= 0.02f;
            }


            transform.position += slideDirection * speed * Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        }
    }
}