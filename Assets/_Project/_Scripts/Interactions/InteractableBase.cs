using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum InteractionMode
{
    Immediate,
    Sequenced
}

[System.Serializable]
public class InteractionSequenceBlock
{
    public float delayTimeBefore = 0f;
    public List<EffectStrategySO> effects;
    public List<FeatureBase> features;
}

public class InteractableBase : MonoBehaviour, IWorldInteractable
{
    [Header("Interaction Setup")]
    [SerializeField] private bool isBlocking = true;
    [SerializeField] private InteractionMode interactionMode = InteractionMode.Immediate;
    [SerializeField] private List<EntryStrategySO> entryStrategies = new();
    [SerializeField] private List<ExitStrategySO> exitStrategies = new();

    [Header("Effects and Features")]
    [SerializeField] private List<EffectStrategySO> effects = new();
    [SerializeField] private List<FeatureBase> features = new();

    [Header("Highlight Settings")]
    [SerializeField] private Light2D highlightLight;
    [SerializeField] private float highlightIntensity = 2.0f;

    private Coroutine highlightRoutine;

    [Header("Optional Sequenced Blocks")]
    [SerializeField] private List<InteractionSequenceBlock> sequenceBlocks = new();

    private int currentBlockIndex = 0;
    private bool sequenceActive = false;
    private bool blockStarted = false;

    private IPuzzleInteractor currentInteractor;
    private bool isInteracting = false;
    private bool hasDocked = false;


    #region Unity Update

    private void Update()
    {
        if (!isInteracting || interactionMode != InteractionMode.Immediate || !isBlocking) return;

        if (AreAllExitStrategiesSatisfied())
        {
            Debug.Log($"[InteractableBase] Exiting immediate interaction on {name}");
            OnInteractionComplete(currentInteractor, true);
        }
    }

    #endregion

    #region IWorldInteractable Implementation

    public virtual string GetDisplayName() => gameObject.name;

    public virtual Transform GetTransform() => transform;

    public bool CanBeInteractedWith(IPuzzleInteractor interactor)
    {
        if (isInteracting) return false;
        foreach (var entry in entryStrategies)
        {
            if (!entry.CanEnter(interactor, this)) return false;
        }
        return true;
    }

    public void OnInteract(IPuzzleInteractor interactor)
    {
        if (!CanBeInteractedWith(interactor)) return;

        currentInteractor = interactor;
        isInteracting = true;

        if (interactionMode == InteractionMode.Immediate)
        {
            foreach (var effect in effects)
                effect?.ApplyEffect(interactor, this, InteractionResult.Success);

            foreach (var feature in features)
                feature?.OnInteract(interactor);

            if (!isBlocking)
                isInteracting = false;
        }
        else
        {
            StartCoroutine(RunSequence());
        }
    }

    private IEnumerator RunSequence()
    {
        sequenceActive = true;
        currentBlockIndex = 0;

        while (currentBlockIndex < sequenceBlocks.Count)
        {
            var block = sequenceBlocks[currentBlockIndex];
            Debug.Log($"[InteractableBase] Executing sequence block {currentBlockIndex} on {name}");

            if (block.delayTimeBefore > 0f)
            {
                Debug.Log($"[InteractableBase] Waiting {block.delayTimeBefore} seconds before executing block {currentBlockIndex}.");
                yield return new WaitForSeconds(block.delayTimeBefore);
            }

            foreach (var effect in block.effects)
            {
                effect?.ApplyEffect(currentInteractor, this, InteractionResult.Success);
            }

            foreach (var feature in block.features)
                feature?.OnInteract(currentInteractor);

            bool waiting = true;
            while (waiting)
            {
                waiting = false;
                foreach (var feature in block.features)
                {
                    if (feature != null && !feature.IsComplete)
                    {
                        waiting = true;
                        break;
                    }
                }
                yield return null;
            }

            currentBlockIndex++;
        }

        Debug.Log($"[InteractableBase] Sequence complete on {name}");

        OnInteractionComplete(currentInteractor, true);
    }

    public virtual void OnInteractionComplete(IPuzzleInteractor actor, bool interactionSucceeded)
    {
        Debug.Log($"[InteractableBase] Interaction completed on {name}. Success: {interactionSucceeded}");

        isInteracting = false;
        sequenceActive = false;
        blockStarted = false;
        currentBlockIndex = 0;
        hasDocked = false;
    }

    public virtual List<ExitStrategySO> GetExitStrategies() => exitStrategies;

    #endregion

    #region Exit Evaluation
    private bool AreAllExitStrategiesSatisfied()
    {
        foreach (var exit in exitStrategies)
        {
            if (!exit.ShouldExit(currentInteractor, this))
                return false;
        }
        return true;
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
    
    #region Hooks

    public virtual Vector3 GetDockPosition()
    {
        DockPointFeature dock = GetComponent<DockPointFeature>();
        return dock != null ? dock.GetDockPosition() : transform.position;
    }

    public void MarkDocked() => hasDocked = true;
    public bool HasDocked() => hasDocked;

    public virtual void BroadcastEvent(string eventId)
    {
        // Future hook for global event relay
    }

    #endregion
}
