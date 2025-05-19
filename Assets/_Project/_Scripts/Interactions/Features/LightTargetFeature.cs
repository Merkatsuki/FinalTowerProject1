using UnityEngine;

public class LightTargetFeature : FeatureBase
{
    [Header("Target Settings")]
    [SerializeField] private bool onlyTriggerOnce = true;
    [SerializeField] private bool hasBeenTriggered = false;
    [SerializeField] private InteractableBase targetInteractable;

    public void OnLightReceived(LensFeature source)
    {
        if (onlyTriggerOnce && hasBeenTriggered) return;

        hasBeenTriggered = true;
        Debug.Log($"[LightTargetFeature] Activated by lens: {source.name}");

        //NotifyPuzzleInteractionSuccess();  For if we end up setting up the puzzle contrller to handle this
        RunFeatureEffects();

        // Optionally trigger attached interactable
        if (targetInteractable != null)
        {
            targetInteractable.OnInteract(null);
        }
    }

    public override void OnInteract(IPuzzleInteractor interactor)
    {
        // Could allow manual activation if desired
        OnLightReceived(null);
    }
}
