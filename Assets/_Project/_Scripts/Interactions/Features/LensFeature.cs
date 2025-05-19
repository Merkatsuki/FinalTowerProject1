using System.Collections.Generic;
using UnityEngine;

public class LensFeature : FeatureBase
{
    [Header("Lens Settings")]
    [SerializeField] private Collider2D lightConeTrigger;
    [SerializeField] private List<LightTargetFeature> possibleTargets;
    [SerializeField] private bool autoActivate = true;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!autoActivate) return;

        foreach (var target in possibleTargets)
        {
            if (other.gameObject == target.gameObject && IsLensAlignedWith(target))
            {
                target.OnLightReceived(this);
            }
        }
    }

    private bool IsLensAlignedWith(LightTargetFeature target)
    {
        // You could do dot-product based angle check, or a simple facing enum match
        return true; // Placeholder logic — consider adding direction constraints later
    }

    public override void OnInteract(IPuzzleInteractor actor)
    {
        // Optional: Rotate the lens on player interaction
        transform.Rotate(0f, 0f, 90f); // Example: rotate 90° on interaction
    }
}
