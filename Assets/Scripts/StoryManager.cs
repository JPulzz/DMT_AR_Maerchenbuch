using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public enum StoryState
    {
        Intro,
        FoxApproaches,
        FoxFlattersCrow,
        PlayerDecision,
        RavenKeepsCheese,
        RavenDropsCheese,
        Ending
    }

    [SerializeField]
    private StoryState currentState = StoryState.Intro;

    public StoryState CurrentState => currentState;

    public void SetState(StoryState newState)
    {
        currentState = newState;
        Debug.Log("Story state changed to: " + currentState);
    }

    public void ChooseWarnRaven()
    {
        SetState(StoryState.RavenKeepsCheese);
    }

    public void ChooseIgnoreFox()
    {
        SetState(StoryState.RavenDropsCheese);
    }

    public void ResetStory()
    {
        SetState(StoryState.Intro);
    }
}