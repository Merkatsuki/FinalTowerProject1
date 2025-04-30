using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class InteractableBase : MonoBehaviour, IWorldInteractable
{
    [Header("Optional: Override Collider for Trigger Checks")]
    [SerializeField] private Collider2D triggerColliderOverride;

    [Header("Strategy Settings")]
    public List<EntryStrategySO> entryStrategies;
    public List<ExitStrategySO> exitStrategies;

    [Header("Highlight Settings")]
    [SerializeField] private Light2D highlightLight;
    [SerializeField] private float highlightIntensity = 2.0f;

    private Coroutine highlightRoutine;
    private bool interactionInProgress = false;
    private IPuzzleInteractor currentActor;
    private bool interactionSuccess = false;

    #region Unity Update

    private void Update()
    {
        if (!interactionInProgress)
            return;

        if (AllExitConditionsMet())
        {
            interactionInProgress = false;
            Debug.Log($"[InteractableBase] Interaction complete. Success: {interactionSuccess}");

            OnInteractionComplete(currentActor, interactionSuccess);
        }
    }

    #endregion

    #region IWorldInteractable Implementation

    public virtual string GetDisplayName() => gameObject.name;

    public virtual Transform GetTransform() => transform;

    public virtual bool CanBeInteractedWith(IPuzzleInteractor actor)
    {
        foreach (var strategy in entryStrategies)
        {
            if (strategy != null && !strategy.CanEnter(actor, this))
                return false;
        }

        return true;
    }

    public virtual void OnInteract(IPuzzleInteractor actor)
    {
        Debug.Log($"[InteractableBase] OnInteract triggered by {actor}");

        currentActor = actor;
        interactionInProgress = true;
        interactionSuccess = true;

        foreach (var feature in GetComponents<IInteractableFeature>())
        {
            feature.OnInteract(actor);
        }
    }

    public virtual void OnInteractionComplete(IPuzzleInteractor actor, bool interactionSucceeded)
    {
        Debug.Log($"[InteractableBase] OnInteractionComplete called manually. Success: {interactionSucceeded}");

        if (interactionInProgress)
        {
            interactionSuccess = interactionSucceeded;
            // Final exit resolution occurs in Update via AllExitConditionsMet
        }

        foreach (var strategy in exitStrategies)
        {
            if (strategy != null)
            {
                strategy.OnExit(actor, this);
            }
        }

        currentActor = null;
    }

    public virtual List<ExitStrategySO> GetExitStrategies() => exitStrategies;

    #endregion

    #region Exit Evaluation

    private bool AllExitConditionsMet()
    {
        if (currentActor == null)
            return false;

        foreach (var strategy in exitStrategies)
        {
            if (strategy != null && strategy.ShouldExit(currentActor, this))
                return true;
        }

        return false;
    }

    public bool ShouldExit(IPuzzleInteractor actor)
    {
        foreach (var strategy in exitStrategies)
        {
            if (strategy != null && strategy.ShouldExit(actor, this))
                return true;
        }
        return false;
    }

    #endregion

    #region Highlight Control

    public void SetHighlightLight(Light2D light)
    {
        highlightLight = light;
    }

    public void SetHighlight(bool enabled)
    {
        if (highlightLight == null) return;

        if (highlightRoutine != null)
            StopCoroutine(highlightRoutine);

        highlightRoutine = StartCoroutine(LerpHighlightIntensity(enabled ? highlightIntensity : 0f));
    }

    private IEnumerator LerpHighlightIntensity(float target)
    {
        float start = highlightLight.intensity;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * 7.5f;
            highlightLight.intensity = Mathf.Lerp(start, target, t);
            yield return null;
        }

        highlightLight.intensity = target;
    }

    #endregion

    #region Collider Override Accessors

    public void SetTriggerColliderOverride(Collider2D col)
    {
        triggerColliderOverride = col;
    }

    public Collider2D GetTriggerCollider()
    {
        return triggerColliderOverride;
    }

    #endregion

    #region Hooks

    public virtual void BroadcastEvent(string eventId)
    {
        // Future hook for global event relay
    }

    #endregion
}
