using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class InteractableBase : MonoBehaviour, IWorldInteractable
{
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

    private void Update()
    {
        if (!interactionInProgress)
            return;

        if (AllExitConditionsMet())
        {
            interactionInProgress = false;
            Debug.Log($"[InteractableBase] Interaction complete. Success: {interactionSuccess}");

            // 🚫 Effects are now handled by Features individually, NOT here.
        }
    }

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

    public virtual List<ExitStrategySO> GetExitStrategies() => exitStrategies;

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

    public void OnInteractionComplete(IPuzzleInteractor actor, bool interactionSucceeded)
    {
        Debug.Log($"[InteractableBase] OnInteractionComplete called manually. Success: {interactionSucceeded}");

        if (interactionInProgress)
        {
            interactionSuccess = interactionSucceeded;
            // We still wait for AllExitConditionsMet() before ending interaction
        }
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

    public virtual void SetHighlight(bool enabled)
    {
        if (highlightLight == null) return;

        if (highlightRoutine != null)
            StopCoroutine(highlightRoutine);

        float target = enabled ? highlightIntensity : 0f;
        highlightRoutine = StartCoroutine(LerpHighlightIntensity(target));
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

    public virtual void BroadcastEvent(string eventId)
    {
        // Future event system hook.
    }

    public void SetHighlightLight(Light2D light)
    {
        highlightLight = light;
    }

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
}
