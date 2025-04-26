using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class InteractableBase : MonoBehaviour, IWorldInteractable
{
    [Header("Strategy Settings")]
    [SerializeField] protected List<EntryStrategySO> entryStrategies;
    [SerializeField] protected List<ExitStrategySO> exitStrategies;

    [Header("Highlight Settings")]
    [SerializeField] private Light2D highlightLight;
    [SerializeField] private float highlightIntensity = 2.0f;

    private Coroutine highlightRoutine;

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
        foreach (var feature in GetComponents<IInteractableFeature>())
        {
            feature.OnInteract(actor);
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
        // Future event system integration
    }
    public void SetHighlightLight(Light2D light)
    {
        highlightLight = light;
    }
}
