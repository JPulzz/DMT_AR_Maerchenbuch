using UnityEngine;

public class RavenAnimationController : MonoBehaviour
{
    private Animator ravenAnimator;

    private void Awake()
    {
        ravenAnimator = GetComponentInChildren<Animator>();
    }

    public void PlayTalk()
    {
        ravenAnimator.SetTrigger("Talk");
    }

    public void PlayFlattered()
    {
        ravenAnimator.SetTrigger("Flattered");
    }

    public void PlayDropCheese()
    {
        ravenAnimator.SetTrigger("DropCheese");
    }

    public void PlayKeepCheese()
    {
        ravenAnimator.SetTrigger("KeepCheese");
    }

}