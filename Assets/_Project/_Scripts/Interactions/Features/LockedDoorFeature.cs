using UnityEngine;
using System.Collections.Generic;

public class LockedDoorFeature : MonoBehaviour, IInteractableFeature
{
    [SerializeField] private bool isLocked = true;
    [SerializeField] private string requiredFlag;

    [Header("Feature Effects")]
    [SerializeField] private List<EffectStrategySO> featureEffects = new();

    private Animator doorAnimator;

    private void Awake()
    {
        doorAnimator = GetComponent<Animator>();
        if (doorAnimator == null)
        {
            doorAnimator = gameObject.AddComponent<Animator>();
            Debug.LogWarning("[LockedDoorFeature] No Animator found. Added default Animator component.");
        }
    }

    public void OnInteract(IPuzzleInteractor actor)
    {
        AttemptUnlock(actor);
    }

    public bool CanUnlock()
    {
        if (!isLocked)
            return true;

        if (string.IsNullOrEmpty(requiredFlag))
            return false;

        return PuzzleManager.Instance != null && PuzzleManager.Instance.IsFlagSet(requiredFlag);
    }

    public void AttemptUnlock(IPuzzleInteractor actor)
    {
        if (CanUnlock())
        {
            Unlock(actor);
        }
        else
        {
            Debug.Log("[LockedDoorFeature] Unlock conditions not met.");
        }
    }

    public void Unlock(IPuzzleInteractor actor)
    {
        isLocked = false;
        Debug.Log("[LockedDoorFeature] Door unlocked.");

        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Open");
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

    public bool IsUnlocked() => !isLocked;
}
