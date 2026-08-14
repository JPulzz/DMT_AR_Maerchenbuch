using UnityEngine;

public class RavenAnimationTest : MonoBehaviour
{
    private Animator ravenAnimator;

    private void Awake()
    {
        ravenAnimator = GetComponent<Animator>();
    }

    public void TriggerReaction()
    {
        ravenAnimator.SetTrigger("React");
    }
}