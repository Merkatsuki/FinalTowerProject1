using UnityEngine;
using System.Collections.Generic;

public class DialogueFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Dialogue Settings")]
    [SerializeField] private string oneLiner;
    [SerializeField] private DialogueSequence dialogueSequence;

    [Header("Feature Effects")]
    [SerializeField] private List<EffectStrategySO> featureEffects = new();

    public void OnInteract(IPuzzleInteractor actor)
    {
        TriggerDialogue(actor);
    }

    public void TriggerDialogue(IPuzzleInteractor actor)
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("[DialogueFeature] No DialogueManager found.");
            return;
        }

        if (dialogueSequence != null && dialogueSequence.lines != null && dialogueSequence.lines.Count > 0)
        {
            DialogueManager.Instance.ShowDialogue(dialogueSequence);
        }
        else if (!string.IsNullOrEmpty(oneLiner))
        {
            DialogueManager.Instance.ShowMessage(oneLiner);
        }
        else
        {
            Debug.LogWarning("[DialogueFeature] No dialogue set.");
        }

        RunFeatureEffects(actor);
    }

    private void RunFeatureEffects(IPuzzleInteractor actor)
    {
        foreach (var effect in featureEffects)
        {
            if (effect != null && TryGetComponent(out IWorldInteractable interactable))
            {
                effect.ApplyEffect(actor, interactable, InteractionResult.Success);
            }
        }
    }

    public void SetFeatureEffects(List<EffectStrategySO> effects)
    {
        featureEffects = effects ?? new List<EffectStrategySO>();
    }

    public bool HasDialogue()
    {
        return dialogueSequence != null || !string.IsNullOrEmpty(oneLiner);
    }
}
