// Updated CollectibleFeature.cs to integrate with InventoryManager and ItemSO
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollectibleFeature : FeatureBase
{
    [Header("Collection Settings")]
    [SerializeField] private ItemSO collectibleItem;
    [SerializeField] private bool disableOnCollect = true;
    [SerializeField] private float dissolveDuration = 1f;
    [SerializeField] private GameObject visualObject;

    [Header("Stage Requirements")]
    [SerializeField] private List<FlagSO> requiredCollectionStages = new();

    [Header("Events")]
    [SerializeField] private UnityEvent onCollect;

    private bool collected = false;

    public override void OnInteract(IPuzzleInteractor actor)
    {
        if (collected) return;
        if (!CanCollect()) return;

        collected = true;

        // Add to inventory
        if (collectibleItem != null)
        {
            InventoryManager.Instance?.AddItem(collectibleItem);
        }

        // Trigger effects
        foreach (var effect in featureEffects)
        {
            if (effect != null && TryGetComponent(out IWorldInteractable interactable))
                effect.ApplyEffect(actor, interactable, InteractionResult.Success);
        }

        onCollect?.Invoke();
        Debug.Log($"[Collectible] Attempting collection of: {collectibleItem?.ItemName}, Already Collected: {collected}");

        if (disableOnCollect && visualObject != null)
            StartCoroutine(DissolveCollectible());
    }

    private bool CanCollect()
    {
        foreach (var stage in requiredCollectionStages)
        {
            if (stage != null && !FlagManager.Instance.GetBool(stage))
                return false;
        }
        return true;
    }

    private System.Collections.IEnumerator DissolveCollectible()
    {
        float elapsed = 0f;
        Renderer rend = visualObject?.GetComponent<Renderer>();
        Material mat = rend?.material;

        while (elapsed < dissolveDuration && mat != null)
        {
            float t = elapsed / dissolveDuration;
            mat.SetFloat("_DissolveAmount", t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (visualObject != null)
            visualObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
