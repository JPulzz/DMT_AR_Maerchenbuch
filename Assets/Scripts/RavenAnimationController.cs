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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            PlayTalk();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            PlayFlattered();

        if (Input.GetKeyDown(KeyCode.Alpha3))
            PlayDropCheese();
    }
}