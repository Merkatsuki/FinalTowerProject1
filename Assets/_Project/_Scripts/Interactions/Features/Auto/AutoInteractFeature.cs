// AutoInteractFeature.cs
using UnityEngine;

public class AutoInteractFeature : AutoTriggerFeatureBase
{
    [SerializeField] private InteractableBase interactableTarget;

    protected override void ExecuteTrigger()
    {
        Debug.Log($"[AutoInteractFeature] Triggering interaction on {gameObject.name}");

        if (interactableTarget != null)
        {
            Debug.Log($"[AutoInteractFeature] Calling OnInteract on target InteractableBase: {gameObject.name}");
            interactableTarget.OnInteract(Interactor);
        }
        else
        {
            Debug.LogWarning($"[AutoInteractFeature] No valid interactableTarget assigned or not IInteractableFeature.");
        }

        RunFeatureEffects();
    }
}
