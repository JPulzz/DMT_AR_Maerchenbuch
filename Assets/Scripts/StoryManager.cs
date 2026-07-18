using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public enum StoryState
    {
        Intro,
        FoxApproaches,
        FoxFlattersCrow,
        PlayerDecision,
        CrowKeepsCheese,
        CrowDropsCheese,
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

    public void ChooseWarnCrow()
    {
        SetState(StoryState.CrowDropsCheese);
    }

    public void ChooseIgnoreFox()
    {
        SetState(StoryState.CrowKeepsCheese);
    }

    public void ResetStory()
    {
        SetState(StoryState.Intro);
    }
}