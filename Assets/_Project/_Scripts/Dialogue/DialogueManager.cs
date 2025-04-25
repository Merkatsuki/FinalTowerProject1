using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private CanvasGroup dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float displayTimePerCharacter = 0.04f;

    private Coroutine currentRoutine;
    private bool isPlaying = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ShowMessage(string message)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(TypeOutDialogue(message));
    }

    private IEnumerator TypeOutDialogue(string message)
    {
        isPlaying = true;
        dialogueBox.alpha = 1;
        dialogueText.text = "";

        foreach (char c in message)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(displayTimePerCharacter);
        }

        yield return new WaitForSeconds(1.5f);
        dialogueBox.alpha = 0;
        isPlaying = false;
    }

    public bool IsDialoguePlaying()
    {
        return isPlaying;
    }

    public void ShowDialogue(DialogueSequence sequence)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(TypeOutSequence(sequence));
    }

    private IEnumerator TypeOutSequence(DialogueSequence sequence)
    {
        isPlaying = true;
        dialogueBox.alpha = 1;
        dialogueText.text = "";

        foreach (var line in sequence.lines)
        {
            dialogueText.text = "";
            foreach (char c in line)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(displayTimePerCharacter);
            }
            yield return new WaitForSeconds(1f);
        }

        if (sequence.pauseAfter)
            yield return new WaitForSeconds(1.5f);

        dialogueBox.alpha = 0;
        isPlaying = false;
    }

}

[System.Serializable]
public class DialogueNode
{
    public string id;
    public string line;
    public List<DialogueChoice> choices;
}

[System.Serializable]
public class DialogueChoice
{
    public string text;
    public string nextNodeId;
}