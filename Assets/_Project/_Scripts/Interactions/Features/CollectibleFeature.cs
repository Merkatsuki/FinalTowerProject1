using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollectibleFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Collection Settings")]
    [SerializeField] private float selfDestructDelay = 0f;
    [SerializeField] private bool useDissolveEffect = false;
    [SerializeField] private DissolveController dissolveController; // Optional !!! Not Tested, Forced, Or Material Made Yet
    [SerializeField] private bool animateToPlayerOnCollect = false;
    [SerializeField] private Transform playerTransform; // Set dynamically if needed
    [SerializeField] private int requiredCollectionStages = 1;
    private int currentCollectionProgress = 0;

    [Header("Effects")]
    [SerializeField] private List<EffectStrategySO> featureEffects = new();

    [Header("Events")]
    [SerializeField] private UnityEvent onCollect;

    private bool collected = false;

    public void OnInteract(IPuzzleInteractor actor)
    {
        if (collected) return;

        currentCollectionProgress++;

        if (currentCollectionProgress < requiredCollectionStages)
        {
            // Could add a partial collection effect here
            return;
        }

        collected = true;

        // Fire UnityEvent for designers
        onCollect?.Invoke();

        // Run assigned Feature Effects
        RunFeatureEffects(actor);

        // Optionally differentiate companion behavior
        if (actor is CompanionController)
        {
            Debug.Log("[CollectibleFeature] Collected by Companion!");
            // Optionally different behavior here (sound, FX, etc.)
        }

        if (animateToPlayerOnCollect && playerTransform != null)
        {
            StartCoroutine(AnimateToPlayerCoroutine());
        }
        else
        {
            StartCollectionVisualEffects();
        }
    }

    private void RunFeatureEffects(IPuzzleInteractor actor)
    {
        foreach (var effect in featureEffects)
        {
            if (effect != null && TryGetComponent(out IWorldInteractable interactable))
            {
                effect.ApplyEffect(actor, interactable, InteractionResult.Success);
            }
        }
    }

    private void StartCollectionVisualEffects()
    {
        if (useDissolveEffect && dissolveController != null)
        {
            dissolveController.TriggerDissolve(() => StartCoroutine(DelayedDestroy()));
        }
        else
        {
            StartCoroutine(DelayedDestroy());
        }
    }

    private IEnumerator DelayedDestroy()
    {
        if (selfDestructDelay > 0f)
        {
            yield return new WaitForSeconds(selfDestructDelay);
        }

        Destroy(gameObject);
    }

    private IEnumerator AnimateToPlayerCoroutine()
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = playerTransform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        StartCollectionVisualEffects();
    }

    // Setter for FeatureEffects
    public void SetFeatureEffects(List<EffectStrategySO> effects)
    {
        featureEffects = effects ?? new List<EffectStrategySO>();
    }

    // Optional setter for playerTransform
    public void SetPlayerTransform(Transform player)
    {
        playerTransform = player;
    }
}
