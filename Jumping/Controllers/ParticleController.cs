using Assets.Scripts.Jumping;
using UnityEngine;


namespace Assets.Scripts.Jumping.Controllers
{
    public class ParticleController : MonoBehaviour
    {
        [SerializeField] ParticleSystem inRunCrash, afterLand, crash;
        [SerializeField] Fly fly;

        private void Play(ParticleSystem p)
        {
            if (!p.isPlaying) p.Play();
        }

        private void Stop(ParticleSystem p)
        {
            p.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        public void PlayInRunHit() => Play(inRunCrash);

        public void StopInRunHit() => Stop(inRunCrash);

        public void PlayAfterLand() => Play(afterLand);

        public void StopAfterLand() => Stop(afterLand);

        public void PlayCrash() => Play(crash);

        public void StopCrash() => Stop(crash);

        private void Update()
        {
            if ((crash.isPlaying || afterLand.isPlaying) && fly.speed < 1f)
            {
                StopCrash();
                StopAfterLand();
            }
        }
    }
}