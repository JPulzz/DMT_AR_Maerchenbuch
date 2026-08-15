using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;


    private void Start()
    {
        dialoguePanel.SetActive(false);
    }

    public void ShowPanel()
    {
        dialoguePanel.SetActive(true);
    }

    public void ShowDialogue(string text)
    {
        dialogueText.text = text;
        dialoguePanel.SetActive(true);
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
    }

}