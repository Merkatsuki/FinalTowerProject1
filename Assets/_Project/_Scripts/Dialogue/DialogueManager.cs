using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public enum DialogueMode { Graph, Sequence, OneLiner }

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Transform choicesContainer;
    [SerializeField] private GameObject choiceButtonPrefab;
    [SerializeField] private float typingSpeed = 0.02f;

    [Header("Typing Settings")]
    [SerializeField] private bool useTypewriter = true;

    private DialogueGraphSO currentGraph;
    private DialogueNode currentNode;
    private bool isTyping = false;
    private bool awaitingChoice = false;
    private System.Action onDialogueComplete; // Optional callback when dialogue ends

    private static DialogueManager instance;
    public static DialogueManager Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Update()
    {
        if (dialoguePanel.activeSelf && !awaitingChoice && Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                SkipTyping();
            }
            else
            {
                AdvanceDialogue();
            }
        }
    }

    #region Public API

    public void StartDialogue(DialogueGraphSO graph, System.Action onComplete = null)
    {
        if (graph == null)
        {
            Debug.LogWarning("Tried to start dialogue with a null graph.");
            return;
        }

        onDialogueComplete = onComplete;
        currentGraph = graph;
        currentNode = graph.GetNodeById(graph.startNodeId);

        if (currentNode == null)
        {
            Debug.LogError("Start Node not found!");
            return;
        }

        dialoguePanel.SetActive(true);
        DisplayNode(currentNode);
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        ClearChoices();
        currentGraph = null;
        currentNode = null;
        onDialogueComplete?.Invoke();
    }

    public void StartDialogueSequence(DialogueSequence sequence, System.Action onComplete = null)
    {
        if (sequence == null || sequence.lines == null || sequence.lines.Count == 0)
        {
            Debug.LogWarning("Tried to start an empty dialogue sequence.");
            return;
        }

        onDialogueComplete = onComplete;

        // Build a temporary dialogue graph on the fly
        DialogueGraphSO tempGraph = ScriptableObject.CreateInstance<DialogueGraphSO>();
        tempGraph.graphName = "Sequence_" + sequence.speaker;
        tempGraph.startNodeId = "0";

        for (int i = 0; i < sequence.lines.Count; i++)
        {
            DialogueNode node = new DialogueNode
            {
                id = i.ToString(),
                speakerName = sequence.speaker,
                lineText = sequence.lines[i],
                choices = new List<DialogueChoice>(),
                autoAdvance = true
            };

            // Optionally add a dummy choice to auto-advance
            if (i < sequence.lines.Count - 1)
            {
                node.choices.Add(new DialogueChoice
                {
                    text = "", // hidden
                    nextNodeId = (i + 1).ToString()
                });
            }

            tempGraph.nodes.Add(node);
        }

        StartDialogue(tempGraph, onComplete);
        Destroy(tempGraph);
    }

    public void ShowOneLiner(string line, System.Action onComplete = null)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            Debug.LogWarning("Tried to show empty one-liner.");
            onComplete?.Invoke();
            return;
        }

        DialogueSequence quickSeq = new DialogueSequence
        {
            speaker = "", // Or "Narrator"
            lines = new List<string> { line },
            pauseAfter = false
        };

        StartDialogueSequence(quickSeq, onComplete);
    }

    public bool IsDialoguePlaying()
    {
        return dialoguePanel != null && dialoguePanel.activeSelf;
    }

    #endregion

    #region Node Handling

    private void DisplayNode(DialogueNode node)
    {
        ClearChoices();

        if (speakerNameText != null)
        {
            speakerNameText.text = node.speakerName;
        }

        if (useTypewriter)
        {
            StartCoroutine(TypeLine(node.lineText));
        }
        else
        {
            dialogueText.text = node.lineText;
        }

        if (node.choices != null && node.choices.Count > 0)
        {
            awaitingChoice = true;
            foreach (var choice in node.choices)
            {
                CreateChoiceButton(choice);
            }
        }
        else
        {
            awaitingChoice = false;
        }
    }

    private void AdvanceDialogue()
    {
        if (currentNode == null)
        {
            EndDialogue();
            return;
        }

        // If there are choices, we wait for player input
        if (currentNode.choices != null && currentNode.choices.Count > 0)
        {
            return;
        }

        // Try to auto-advance if flagged
        if (currentNode.autoAdvance)
        {
            // Check if a next node ID exists in the first choice
            if (currentNode.choices != null && currentNode.choices.Count == 1)
            {
                string nextId = currentNode.choices[0].nextNodeId;

                if (!string.IsNullOrEmpty(nextId))
                {
                    OnChoiceSelected(nextId);
                    return;
                }
            }
        }

        // If no valid next node, end dialogue
        EndDialogue();
    }


    private void OnChoiceSelected(string nextNodeId)
    {
        var nextNode = currentGraph.GetNodeById(nextNodeId);
        if (nextNode == null)
        {
            Debug.LogError($"Next node with ID {nextNodeId} not found!");
            EndDialogue();
            return;
        }

        currentNode = nextNode;
        DisplayNode(currentNode);
    }

    #endregion

    #region Typing

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

    private void SkipTyping()
    {
        StopAllCoroutines();
        dialogueText.text = currentNode.lineText;
        isTyping = false;
    }

    #endregion

    #region Choice UI

    private void CreateChoiceButton(DialogueChoice choice)
    {
        GameObject buttonObj = Instantiate(choiceButtonPrefab, choicesContainer);
        TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
        Button button = buttonObj.GetComponent<Button>();

        if (buttonText != null)
        {
            buttonText.text = choice.text;
        }

        button.onClick.AddListener(() =>
        {
            awaitingChoice = false;
            OnChoiceSelected(choice.nextNodeId);
        });
    }

    private void ClearChoices()
    {
        foreach (Transform child in choicesContainer)
        {
            Destroy(child.gameObject);
        }
    }

    #endregion
}
