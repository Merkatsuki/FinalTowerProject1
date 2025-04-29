using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class PuzzleUnlockFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Puzzle Unlock Settings")]
    [SerializeField] private bool isLocked = true;
    public UnityEvent onUnlocked;

    [Header("Feature Effects")]
    [SerializeField] private List<EffectStrategySO> featureEffects = new();

    public void OnInteract(IPuzzleInteractor actor)
    {
        AttemptUnlock(actor);
    }

    public void AttemptUnlock(IPuzzleInteractor actor)
    {
        if (isLocked)
        {
            Unlock(actor);
        }
        else
        {
            Debug.Log("[PuzzleUnlockFeature] Already unlocked.");
        }
    }

    public void Unlock(IPuzzleInteractor actor)
    {
        isLocked = false;
        Debug.Log("[PuzzleUnlockFeature] Puzzle unlocked!");
        onUnlocked?.Invoke();
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

    public bool IsUnlocked() => !isLocked;
}
