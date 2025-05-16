// AutoTriggerFeature.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class AutoTriggerFeatureBase : MonoBehaviour
{
    [Header("Auto Trigger Base Settings")]
    [SerializeField] protected bool triggerOnce = true;
    [SerializeField] protected bool repeatable = false;
    [SerializeField] protected float delayBeforeTrigger = 0f;
    [SerializeField] protected List<EffectStrategySO> featureEffects = new();

    protected bool hasTriggered = false;
    protected IPuzzleInteractor Interactor;
    private Coroutine triggerCoroutine;

    public virtual void OnPlayerEnterZone(IPuzzleInteractor interactor)
    {
        Interactor = interactor;
        Debug.Log($"[AutoTrigger] Player entered zone on {gameObject.name}");

        if (triggerCoroutine == null)
        {
            Debug.Log($"[AutoTrigger] Starting trigger coroutine (delay: {delayBeforeTrigger})");
            triggerCoroutine = StartCoroutine(TryTriggerAfterDelay());
        }
    }

    public virtual void OnPlayerExitZone()
    {
        // Reset for repeatable use, if enabled
        if (repeatable && !triggerOnce)
        {
            hasTriggered = false;
        }
    }

    private IEnumerator TryTriggerAfterDelay()
    {
        if ((triggerOnce && hasTriggered) || (!repeatable && hasTriggered))
        {
            Debug.Log($"[AutoTrigger] Trigger already fired, skipping: {gameObject.name}");
            yield break;
        }

        if (delayBeforeTrigger > 0f)
        {
            Debug.Log($"[AutoTrigger] Waiting {delayBeforeTrigger} seconds before triggering.");
            yield return new WaitForSeconds(delayBeforeTrigger);
        }

        hasTriggered = true;
        Debug.Log($"[AutoTrigger] Executing trigger on {gameObject.name}");
        ExecuteTrigger();
        triggerCoroutine = null;
    }

    protected abstract void ExecuteTrigger();

    protected void RunFeatureEffects()
    {
        foreach (var effect in featureEffects)
        {
            if (effect != null && TryGetComponent(out IWorldInteractable interactable))
            {
                effect.ApplyEffect(Interactor, interactable, InteractionResult.Success);
            }
        }
    }

    public void SetFeatureEffects(List<EffectStrategySO> effects)
    {
        featureEffects = effects ?? new List<EffectStrategySO>();
    }
}
