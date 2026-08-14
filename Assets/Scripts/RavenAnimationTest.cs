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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            TriggerReaction();
        }
    }
}