using Unity.VisualScripting;
using UnityEngine;

public enum Landing
{
    Crash,
    Bad,
    Ok,
    Nice
}

public class AnimationControler : MonoBehaviour
{
    [SerializeField] Animator animator;

    public void RunStart() {
        animator.CrossFade("Start",0.01f);
    }

    public void RunUp()
    {
        animator.CrossFade("Up", 0.01f);
    }

    public void RunDown(Landing landing)
    {
        animator.CrossFade(landing.ToString(), 0.01f);
    }

    public void RunIdle() {
        animator.CrossFade("IdleStart", 0.01f);
    }

    public void RunCrashLeft() {
        animator.CrossFade("SkiCrashL", 0.01f);
    }

    public void RunCrashRight()
    {
        animator.CrossFade("SkiCrashR", 0.1f);
    }
}
