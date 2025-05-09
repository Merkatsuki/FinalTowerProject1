// AutoInteractFeature.cs
using UnityEngine;

public class AutoInteractFeature : AutoTriggerFeature
{
    [SerializeField] private MonoBehaviour interactableTarget;

    protected override void ExecuteTrigger()
    {
        if (interactableTarget != null && interactableTarget is IInteractableFeature feature)
        {
            feature.OnInteract(null); // No actor passed; assumes safe to null
        }
        RunFeatureEffects();
    }
}