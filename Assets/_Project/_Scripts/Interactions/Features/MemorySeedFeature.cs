using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Momentum;

public class MemorySeedFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Seed Activation Settings")]
    [SerializeField] private bool requireProximity = true;
    [SerializeField] private float activationDistance = 3f;
    [SerializeField] private bool requireEnergyCharge = false; // Future upgrade option
    [SerializeField] private bool requireLightExposure = false; // Future upgrade option
    [SerializeField] private float growDelay = 1.5f;

    [Header("Growth Visuals")]
    [SerializeField] private GameObject growthPrefab; // What grows after activation
    [SerializeField] private Transform growthSpawnPoint;
    [SerializeField] private bool destroySeedAfterGrowth = true;

    [Header("Feature Effects")]
    [SerializeField] private List<EffectStrategySO> featureEffects = new();

    private bool activated = false;
    private Transform playerTransform;
    private Tween growthTween;

    private void Awake()
    {
        playerTransform = FindFirstObjectByType<Player>()?.transform;
        if (playerTransform == null)
        {
            Debug.LogWarning("[MemorySeedFeature] Player not found in scene!");
        }
    }

    private void Update()
    {
        if (activated) return;

        if (requireProximity && playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (distance <= activationDistance)
            {
                ActivateSeed();
            }
        }
    }

    public void OnInteract(IPuzzleInteractor actor)
    {
        if (activated) return;

        // Optional: allow deliberate interaction if proximity isn't required
        if (!requireProximity)
        {
            ActivateSeed();
        }
    }

    private void ActivateSeed()
    {
        activated = true;

        Debug.Log("[MemorySeedFeature] Seed activated!");

        if (growthPrefab != null)
        {
            GameObject grownObject = Instantiate(growthPrefab, growthSpawnPoint != null ? growthSpawnPoint.position : transform.position, Quaternion.identity);

            if (grownObject.TryGetComponent(out Transform grownTransform))
            {
                grownTransform.localScale = Vector3.zero;
                growthTween = grownTransform.DOScale(Vector3.one, growDelay).SetEase(Ease.OutBack);
            }
        }

        RunFeatureEffects();

        if (destroySeedAfterGrowth)
        {
            Destroy(gameObject, growDelay);
        }
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
}

