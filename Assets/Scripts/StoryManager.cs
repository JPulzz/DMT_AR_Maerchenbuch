using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public enum StoryState
    {
        Intro,
        FoxApproaches,
        FoxFlattersRaven,
        PlayerDecision,
        RavenKeepsCheese,
        RavenDropsCheese,
        Ending
    }

    [SerializeField] private StoryState currentState = StoryState.Intro;
    [SerializeField] private DialogueUI dialogueUI;

    private string[] currentDialogue;
    private int dialogueIndex = 0;

    public StoryState CurrentState => currentState;

    private void Start()
    {
        SetState(StoryState.Intro);
    }

    public void SetState(StoryState newState)
    {
        currentState = newState;
        SetupCurrentState();
    }

    private void SetupCurrentState()
    {
        dialogueIndex = 0;

        switch (currentState)
        {
            case StoryState.Intro:
                currentDialogue = new string[]
                {
                    "Der Rabe sitzt auf dem Baum und hält ein Stück Käse."
                };
                break;

            case StoryState.FoxApproaches:
                currentDialogue = new string[]
                {
                    "Der Fuchs nähert sich dem Raben.",
                    "Er entdeckt den Käse in seinem Schnabel."
                };
                break;

            case StoryState.FoxFlattersRaven:
                currentDialogue = new string[]
                {
                    "Fuchs: Was für ein prächtiger Rabe!",
                    "Fuchs: Deine Stimme muss bestimmt ebenso beeindruckend sein.",
                    "Rabe: ..."
                };
                break;

            case StoryState.PlayerDecision:
                currentDialogue = new string[]
                {
                    "Wie soll der Rabe reagieren?"
                };
                break;

            case StoryState.RavenKeepsCheese:
                currentDialogue = new string[]
                {
                    "Der Rabe bleibt skeptisch.",
                    "Er behält den Käse."
                };
                break;

            case StoryState.RavenDropsCheese:
                currentDialogue = new string[]
                {
                    "Der Rabe lässt sich von der Schmeichelei überzeugen.",
                    "Er öffnet den Schnabel."
                };
                break;

            case StoryState.Ending:
                currentDialogue = new string[]
                {
                    "Die Geschichte endet."
                };
                break;
        }

        ShowCurrentDialogue();
    }

    private void ShowCurrentDialogue()
    {
        if (currentDialogue == null || currentDialogue.Length == 0)
        {
            return;
        }

        dialogueUI.ShowDialogue(currentDialogue[dialogueIndex]);
    }

    public void AdvanceStory()
    {
        if (currentDialogue == null || currentDialogue.Length == 0)
        {
            return;
        }

        if (dialogueIndex < currentDialogue.Length - 1)
        {
            dialogueIndex++;
            ShowCurrentDialogue();
            return;
        }

        AdvanceToNextState();
    }

    private void AdvanceToNextState()
    {
        switch (currentState)
        {
            case StoryState.Intro:
                SetState(StoryState.FoxApproaches);
                break;

            case StoryState.FoxApproaches:
                SetState(StoryState.FoxFlattersRaven);
                break;

            case StoryState.FoxFlattersRaven:
                SetState(StoryState.PlayerDecision);
                break;

            case StoryState.PlayerDecision:
                break;

            case StoryState.RavenKeepsCheese:
            case StoryState.RavenDropsCheese:
                SetState(StoryState.Ending);
                break;

            case StoryState.Ending:
                break;
        }
    }

    public void ChooseKeepCheese()
    {
        if (currentState != StoryState.PlayerDecision)
        {
            return;
        }

        SetState(StoryState.RavenKeepsCheese);
    }

    public void ChooseDropCheese()
    {
        if (currentState != StoryState.PlayerDecision)
        {
            return;
        }

        SetState(StoryState.RavenDropsCheese);
    }

    public void ResetStory()
    {
        SetState(StoryState.Intro);
    }
}