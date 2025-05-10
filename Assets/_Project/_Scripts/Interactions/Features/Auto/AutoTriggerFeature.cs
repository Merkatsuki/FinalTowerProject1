// AutoTriggerFeature.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class AutoTriggerFeature : MonoBehaviour
{
    [Header("Auto Trigger Base Settings")]
    [SerializeField] protected bool triggerOnce = true;
    [SerializeField] protected bool repeatable = false;
    [SerializeField] protected float delayBeforeTrigger = 0f;
    [SerializeField] protected List<EffectStrategySO> featureEffects = new();

    protected bool hasTriggered = false;
    protected IPuzzleInteractor playerInteractor;
    private Coroutine triggerCoroutine;

    public virtual void OnPlayerEnterZone(IPuzzleInteractor interactor)
    {
        playerInteractor = interactor;

        if (triggerCoroutine == null)
            triggerCoroutine = StartCoroutine(TryTriggerAfterDelay());
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
            yield break;

        if (delayBeforeTrigger > 0f)
            yield return new WaitForSeconds(delayBeforeTrigger);

        hasTriggered = true;
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
                effect.ApplyEffect(playerInteractor, interactable, InteractionResult.Success);
            }
        }
    }

    public void SetFeatureEffects(List<EffectStrategySO> effects)
    {
        featureEffects = effects ?? new List<EffectStrategySO>();
    }
}
