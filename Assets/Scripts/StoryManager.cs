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

    private CheeseController cheeseController;

    private string[] currentDialogue;
    private int dialogueIndex = 0;

    public StoryState CurrentState => currentState;

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
        "Auf einem Ast bewacht ein Rabe ein Stück Käse."
                };
                break;

            case StoryState.FoxApproaches:
                currentDialogue = new string[]
                {
        "Ein Fuchs entdeckt den Raben und den Käse.",
        "Er überlegt, wie er an den Käse gelangen kann."
                };
                break;

            case StoryState.FoxFlattersRaven:
                currentDialogue = new string[]
                {
        "Fuchs: Was für ein prächtiger Rabe!",
        "Fuchs: Ein so beeindruckender Vogel muss doch auch eine wunderschöne Stimme haben.",
        "Der Rabe fühlt sich geschmeichelt.",
        "Rabe: Du hältst meine Stimme wirklich für so schön?"
                };
                break;

            case StoryState.PlayerDecision:
                currentDialogue = new string[]
                {
        "Wie soll der Rabe reagieren?",
        "Tippe den Raben an, damit er den Käse vom Ast stößt, oder nähere dich ihm, damit er misstrauisch wird."
                };
                break;

            case StoryState.RavenKeepsCheese:
                currentDialogue = new string[]
                {
        "Der Rabe wird misstrauisch und behält den Käse.",
        "Er frisst ihn selbst.",
        "Doch der Käse war vergiftet.",
        "Auch Misstrauen schützt nicht vor einer Gefahr, die man nicht erkennt."
                };
                break;

            case StoryState.RavenDropsCheese:
                currentDialogue = new string[]
                {
        "Der Rabe lässt sich beeinflussen und stößt den Käse vom Ast.",
        "Der Fuchs schnappt sich den Käse und frisst ihn.",
        "Doch der Käse war vergiftet.",
        "Wer andere aus Gier manipuliert, kann selbst zum Opfer seiner Absichten werden."
                };
                break;

            case StoryState.Ending:
                currentDialogue = new string[]
                {
        "Die Geschichte ist zu Ende."
                };
                break;
        }

        ShowCurrentDialogue();
    }

    private void ShowCurrentDialogue()
    {
        if (currentDialogue == null || currentDialogue.Length == 0)
            return;

        dialogueUI.ShowDialogue(currentDialogue[dialogueIndex]);

        HandleDialogueAnimation();
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
            return;

        if (cheeseController == null)
            cheeseController = FindAnyObjectByType<CheeseController>();

        if (cheeseController != null)
            cheeseController.DropCheese();

        SetState(StoryState.RavenDropsCheese);
    }

    public void ResetStory()
    {
        SetState(StoryState.Intro);
    }

    private void HandleDialogueAnimation()
    {
        RavenAnimationController raven =
            FindAnyObjectByType<RavenAnimationController>();

        if (raven == null)
            return;

        if (currentState == StoryState.FoxFlattersRaven)
        {
            // "Der Rabe fühlt sich geschmeichelt."
            if (dialogueIndex == 2)
            {
                raven.PlayFlattered();
            }

            // "Du hältst meine Stimme wirklich für so schön?"
            if (dialogueIndex == 3)
            {
                raven.PlayTalk();
            }
        }
    }

    public void StartStory()
    {
        dialogueUI.ShowPanel();
        SetState(StoryState.Intro);
    }
}