// DialogueFeature.cs
using UnityEngine;
using System.Collections.Generic;

public class DialogueFeature : FeatureBase
{
    [Header("Dialogue Settings")]
    [Tooltip("If provided, this will override the inline sequence below")]
    [SerializeField] private DialogueSequenceSO sequenceAsset;
    [Tooltip("Optional inline sequence if no asset is used")]
    [SerializeField] private DialogueSequence inlineSequence = new();

    [Header("One Liner (used if no sequence or SO)")]
    [SerializeField] private string oneLinerText;
    [SerializeField] private string oneLinerSpeaker;
    [SerializeField] private bool oneLinerWaitForInput = true;
    [SerializeField] private float oneLinerPauseAfter = 0f;

    private bool dialogueStarted = false;

    private void OnValidate()
    {
        if (sequenceAsset != null)
        {
            inlineSequence.lines.Clear();
            oneLinerText = string.Empty;
        }
        else if (inlineSequence.lines.Count > 0)
        {
            oneLinerText = string.Empty;
        }
    }

    public override void OnInteract(IPuzzleInteractor actor)
    {
        if (dialogueStarted) return;
        dialogueStarted = true;

        if (sequenceAsset != null)
        {
            DialogueManager.Instance.PlaySequence(sequenceAsset, OnDialogueComplete);
        }
        else if (inlineSequence != null && inlineSequence.lines.Count > 0)
        {
            DialogueManager.Instance.PlaySequence(inlineSequence, OnDialogueComplete);
        }
        else if (!string.IsNullOrEmpty(oneLinerText))
        {
            DialogueManager.Instance.PlaySequence(new DialogueSequence
            {
                lines = new List<DialogueLine> {
                    new DialogueLine {
                        speaker = oneLinerSpeaker,
                        text = oneLinerText,
                        waitForInput = oneLinerWaitForInput,
                        pauseAfter = oneLinerPauseAfter
                    }
                }
            }, OnDialogueComplete);
        }
        else
        {
            OnDialogueComplete();
        }
    }

    private void OnDialogueComplete()
    {
        RunFeatureEffects();
        dialogueStarted = false;
    }

    private void RunFeatureEffects()
    {
        foreach (var effect in featureEffects)
        {
            if (effect != null && TryGetComponent(out IWorldInteractable interactable))
            {
                effect.ApplyEffect(null, interactable, InteractionResult.Success);
            }
        }
    }
}
