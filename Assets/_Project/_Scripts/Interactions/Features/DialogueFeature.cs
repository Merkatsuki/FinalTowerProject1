using UnityEngine;
using System.Collections.Generic;

public class DialogueFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Dialogue Settings")]
    [SerializeField] private DialogueGraphSO dialogueGraph;
    [SerializeField] private DialogueSequence dialogueSequence;
    [SerializeField][TextArea(2, 4)] private string oneLiner;

    [Header("Feature Effects")]
    [SerializeField] private List<EffectStrategySO> featureEffects = new();

    private bool dialogueStarted = false;

    public void OnInteract(IPuzzleInteractor actor)
    {
        if (dialogueStarted) return;
        dialogueStarted = true;

        if (dialogueGraph != null)
        {
            // Start Branching Dialogue
            DialogueManager.Instance.StartDialogue(dialogueGraph, OnDialogueComplete);
        }
        else if (dialogueSequence != null)
        {
            // Start Linear Dialogue Sequence
            DialogueManager.Instance.StartDialogueSequence(dialogueSequence, OnDialogueComplete);
        }
        else if (!string.IsNullOrEmpty(oneLiner))
        {
            // Start One Liner
            DialogueManager.Instance.ShowOneLiner(oneLiner, OnDialogueComplete);
        }
        else
        {
            Debug.LogWarning("[DialogueFeature] No dialogue data assigned!");
            dialogueStarted = false;
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

    public void SetFeatureEffects(List<EffectStrategySO> effects)
    {
        featureEffects = effects ?? new List<EffectStrategySO>();
    }
}
