using Assets.Scripts.Audio;
using Assets.Scripts.Jumping.Controllers;
using Assets.Scripts.Jumping.StaticInfo;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Animation
{
    /// <summary>
    /// name use in animator
    /// </summary>
    public enum Landing
    {
        Crash,
        WithTouch,
        TwoLegs,
        Telemark
    }

    public class AnimationControler : MonoBehaviour
    {
        [SerializeField] Animator animator;
        [SerializeField] JumperInfoGlobal jumperInfo;
        [SerializeField] ParticleController particleController;
        [SerializeField] AudioController audioController;

        private void Start()
        {
        }

        public void RunStart()
        {
            animator.CrossFade("Start", 0.01f);
            jumperInfo.jumpState = JumpState.InRun;
        }

        public void RunUp()
        {
            animator.CrossFade("Up", 0.01f);
            jumperInfo.jumpState = JumpState.Fly;
        }

        public void RunDown(Landing landing)
        {
            animator.CrossFade(landing.ToString(), 0.01f);
            jumperInfo.jumpState = JumpState.Down;
            jumperInfo.landingStyle = landing;
        }

        public void RunIdle()
        {
            animator.CrossFade("IdleStart", 0.01f);
            jumperInfo.jumpState = JumpState.Idle;
        }

        public void RunCrashLeft()
        {
            animator.CrossFade("SkiCrashL", 0.01f);
            particleController.PlayCrash();
            audioController.PlayCrash();
        }

        public void RunCrashRight()
        {
            animator.CrossFade("SkiCrashR", 0.1f);
            particleController.PlayCrash();
            audioController.PlayCrash();
        }
    }
}