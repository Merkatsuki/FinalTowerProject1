using UnityEngine;
using System.Collections.Generic;

public class DialogueFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Dialogue Settings")]
    [SerializeField] private DialogueMode mode;
    [SerializeField] private DialogueGraphSO dialogueGraph;
    [SerializeField] private DialogueSequence dialogueSequence;
    [SerializeField][TextArea(2, 4)] private string oneLiner;

    [Header("Feature Effects")]
    [SerializeField] private List<EffectStrategySO> featureEffects = new();

    private bool dialogueStarted = false;

    private void OnValidate()
    {
        switch (mode)
        {
            case DialogueMode.Graph:
                dialogueSequence = null;
                oneLiner = string.Empty;
                break;
            case DialogueMode.Sequence:
                dialogueGraph = null;
                oneLiner = string.Empty;
                break;
            case DialogueMode.OneLiner:
                dialogueGraph = null;
                dialogueSequence = null;
                break;
        }
    }

    public void OnInteract(IPuzzleInteractor actor)
    {
        if (dialogueStarted) return;
        dialogueStarted = true;

        switch (mode)
        {
            case DialogueMode.Graph:
                if (dialogueGraph)
                    DialogueManager.Instance.StartDialogue(dialogueGraph, OnDialogueComplete);
                break;

            case DialogueMode.Sequence:
                if (dialogueSequence != null)
                    DialogueManager.Instance.StartDialogueSequence(dialogueSequence, OnDialogueComplete);
                break;

            case DialogueMode.OneLiner:
                if (!string.IsNullOrEmpty(oneLiner))
                    DialogueManager.Instance.ShowOneLiner(oneLiner, OnDialogueComplete);
                break;
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
