using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Audio
{

    public class AudioController : MonoBehaviour
    {
        [SerializeField] AudioClip start, inrunStart, inrunLoop, fly, land, crash, down;
        [SerializeField] AudioSource jumper, onlyInRunHit;
        private float default_vol = 0.78f;
        private void Start()
        {
            default_vol = jumper.volume;
        }

        public void PlayStart()
        {
            StopAllCoroutines();
            ChangeVolume(default_vol);
            jumper.clip = start;
            jumper.loop = false;
            jumper.Play();
        }

        public void PlayInrun()
        {
            ChangeVolume(default_vol);
            jumper.clip = inrunStart;
            jumper.loop = false;
            jumper.Play();
            StartCoroutine(PlayInLoop(inrunLoop));
        }

        public void PlayInRunHits()
        {
            if (!onlyInRunHit.isPlaying) onlyInRunHit.Play();
        }

        public void StopInRunHits()
        {
            if (onlyInRunHit.isPlaying) onlyInRunHit.Stop();
        }

        public void PlayJump()
        {
            StopAllCoroutines();
            jumper.Stop();
            jumper.clip = fly;
            jumper.loop = true;
            jumper.Play();
        }

        public void PlayCrash()
        {
            StopAllCoroutines();
            jumper.Stop();
            jumper.clip = crash;
            jumper.loop = false;
            jumper.Play();
            StartCoroutine(PlayInLoop(down));
        }

        public void PlayLand()
        {
            if (jumper.clip == land || jumper.clip == down || jumper.clip == crash)
                return;
            StopAllCoroutines();
            jumper.Stop();
            jumper.clip = land;
            jumper.loop = false;
            jumper.Play();
            StartCoroutine(PlayInLoop(down));
        }

        public void StopLand()
        {
            StopAllCoroutines();
            jumper.Stop();
        }

        private IEnumerator PlayInLoop(AudioClip clip)
        {
            yield return new WaitForSeconds(jumper.clip.length);
            jumper.clip = clip;
            jumper.loop = true;
            jumper.Play();

        }

        public void ChangeVolume(float volume)
        {
            jumper.volume = volume;
        }

    }
}