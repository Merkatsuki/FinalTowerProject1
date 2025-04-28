using UnityEngine;

public abstract class EffectStrategySO : ScriptableObject
{
    [Header("Smart Execution Settings")]
    [SerializeField] private bool onlyOnSuccess = true;

    public void ApplyEffect(IPuzzleInteractor actor, IWorldInteractable interactable, InteractionResult result)
    {
        if (onlyOnSuccess && result != InteractionResult.Success)
            return;

        ApplyEffectInternal(actor, interactable, result);
    }

    protected abstract void ApplyEffectInternal(IPuzzleInteractor actor, IWorldInteractable interactable, InteractionResult result);
}
