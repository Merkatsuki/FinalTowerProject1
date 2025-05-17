using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Momentum;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject dialogueModeIndicator;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private float typingSpeed = 0.02f;

    private bool isTyping = false;
    private bool awaitingInput = false;
    private System.Action onDialogueComplete;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        if (InputManager.instance != null)
        {
            InputManager.instance.JumpPressedDialogue += OnAdvanceInput;
        }
    }


    private void OnAdvanceInput()
    {
        if (awaitingInput)
        {
            awaitingInput = false;
        }
    }

    public void PlaySequence(DialogueSequence sequence, System.Action onComplete = null)
    {
        if (sequence == null || sequence.lines == null || sequence.lines.Count == 0)
        {
            Debug.LogWarning("Empty dialogue sequence.");
            return;
        }

        onDialogueComplete = onComplete;
        dialoguePanel.SetActive(true);

        // Dialogue Mode auto-detect logic
        bool shouldBlockInput = sequence.lines.Exists(l => l.waitForInput);
        if (shouldBlockInput)
        {
            InputManager.instance?.SetDialogueMode(true);
            if (dialogueModeIndicator != null)
                dialogueModeIndicator.SetActive(true);
        }

        StartCoroutine(PlaySequenceCoroutine(sequence.lines, shouldBlockInput));
    }

    public void PlaySequence(DialogueSequenceSO sequenceSO, System.Action onComplete = null)
    {
        if (sequenceSO == null || sequenceSO.lines == null || sequenceSO.lines.Count == 0)
        {
            Debug.LogWarning("Empty dialogue asset.");
            return;
        }

        DialogueSequence runtime = new DialogueSequence { lines = sequenceSO.lines };
        PlaySequence(runtime, onComplete);
    }

    private IEnumerator PlaySequenceCoroutine(List<DialogueLine> lines, bool blockInput)
    {
        foreach (var line in lines)
        {
            speakerNameText.text = line.speaker;
            yield return TypeLine(line.text);

            if (line.pauseAfter > 0f)
            {
                yield return new WaitForSeconds(line.pauseAfter);
            }

            if (line.waitForInput)
            {
                awaitingInput = true;
                yield return new WaitUntil(() => !awaitingInput);
            }
        }

        EndDialogue();
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public void EndDialogue()
    {
        if (dialogueModeIndicator != null)
            dialogueModeIndicator.SetActive(false);
        dialoguePanel.SetActive(false);
        speakerNameText.text = "";
        dialogueText.text = "";
        InputManager.instance?.SetDialogueMode(false);
        onDialogueComplete?.Invoke();
    }

    public bool IsDialoguePlaying()
    {
        return dialoguePanel.activeSelf;
    }
}
