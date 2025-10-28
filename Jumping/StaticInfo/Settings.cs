using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;


namespace Assets.Scripts.Jumping.StaticInfo
{
    [CreateAssetMenu(fileName = "Settings", menuName = "Data/Settings")]
    public class Settings : ScriptableObject
    {
        public AudioMixer audioMixer;

        private float mouseXSens;
        private float mouseYSens;
        private float soundVolume;

        public float MouseXSens
        {
            get { return mouseXSens; }
            set { mouseXSens = Mathf.Clamp01(value); }
        }
        public float MouseYSens
        {
            get { return mouseYSens; }
            set { mouseYSens = Mathf.Clamp01(value); }
        }
        public float SoundVolume
        {
            get { return soundVolume; }
            set
            {
                soundVolume = Mathf.Clamp(value, 0.0001f, 1f);
                if (audioMixer != null)
                {
                    float dB = Mathf.Log10(soundVolume) * 20f;
                    audioMixer.SetFloat("Volume", dB);
                }
            }
        }

        public void SetData(Settings data)
        {
            this.MouseXSens = data.mouseXSens;
            this.MouseYSens = data.mouseYSens;
            this.SoundVolume = data.soundVolume;
        }

    }
}