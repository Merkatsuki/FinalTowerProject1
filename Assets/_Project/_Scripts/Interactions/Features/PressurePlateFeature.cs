using UnityEngine;
using System.Collections.Generic;

public class PressurePlateFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Plate Settings")]
    [SerializeField] private List<string> acceptedTags = new() { "Player", "Companion" }; // Who can trigger
    [SerializeField] private bool staysActivated = true; // Stays on after triggering?
    [SerializeField] private bool allowMultipleTriggers = false; // Fire every time stepped on?

    [Header("Unlock Target")]
    [SerializeField] private List<GameObject> unlockTargets = new(); // What gets activated

    [Header("Feature Effects")]
    [SerializeField] private List<EffectStrategySO> featureEffects = new();

    private bool activated = false;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider2D>();
            ((BoxCollider2D)col).isTrigger = true;
        }
        else
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated && !allowMultipleTriggers) return;

        if (acceptedTags.Contains(other.tag))
        {
            ActivatePlate();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!staysActivated && acceptedTags.Contains(other.tag))
        {
            DeactivatePlate();
        }
    }

    private void ActivatePlate()
    {
        activated = true;

        foreach (var target in unlockTargets)
        {
            if (target != null)
            {
                target.SetActive(true);
            }
        }

        RunFeatureEffects();
    }

    private void DeactivatePlate()
    {
        activated = false;

        foreach (var target in unlockTargets)
        {
            if (target != null)
            {
                target.SetActive(false);
            }
        }

        // (Optional: trigger different effects on deactivate later if needed)
    }

    private void RunFeatureEffects()
    {
        foreach (var effect in featureEffects)
        {
            if (effect != null && TryGetComponent(out IWorldInteractable interactable))
            {
                effect.ApplyEffect(null, interactable, InteractionResult.Success);
            }
        }
    }

    public void SetFeatureEffects(List<EffectStrategySO> effects)
    {
        featureEffects = effects ?? new List<EffectStrategySO>();
    }

    public void OnInteract(IPuzzleInteractor actor)
    {
        Debug.LogWarning("[PressurePlateFeature] Pressure Plates do not require direct interaction. They are triggered by proximity!");
    }
}


