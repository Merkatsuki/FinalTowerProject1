using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class PuzzleUnlockFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Unlock Targets")]
    [SerializeField] private List<GameObject> unlockTargets = new(); // Multiple unlockables supported

    [Header("Puzzle Progression")]
    [SerializeField] private FlagSO unlockFlag; // Optional flag set

    [Header("Unlock Events")]
    [SerializeField] private UnityEvent onUnlocked; // Optional for designers

    [Header("Feature Effects")]
    [SerializeField] private List<EffectStrategySO> featureEffects = new();

    private bool unlocked = false;

    public void OnInteract(IPuzzleInteractor actor)
    {
        if (unlocked) return;

        Unlock();
    }

    private void Unlock()
    {
        unlocked = true;

        foreach (var target in unlockTargets)
        {
            if (target != null)
            {
                target.SetActive(true);
            }
        }

        if (unlockFlag != null)
        {
            PuzzleManager.Instance.SetFlag(unlockFlag, true);
        }

        onUnlocked?.Invoke();

        RunFeatureEffects();
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
