using UnityEngine;
using TMPro;
using System.Collections;

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
}