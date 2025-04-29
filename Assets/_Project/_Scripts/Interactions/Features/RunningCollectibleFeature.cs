using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Momentum;

public class RunningCollectibleFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Movement Settings")]
    [SerializeField] private float fleeDistance = 5f;
    [SerializeField] private float fleeSpeed = 5f;
    [SerializeField] private float activationRange = 3f;
    [SerializeField] private float randomFleeAngleVariance = 45f;
    [SerializeField] private float fleeCooldown = 2f;

    [Header("Capture Settings")]
    [SerializeField] private float captureRange = 1f;
    [SerializeField] private GameObject collectibleVisual;

    [Header("Feature Effects")]
    [SerializeField] private List<EffectStrategySO> featureEffects = new();

    private bool fleeing = false;
    private bool captured = false;
    private Transform playerTransform;
    private Tween moveTween;
    private float lastFleeTime;

    private void Awake()
    {
        playerTransform = FindFirstObjectByType<Player>()?.transform;
        if (playerTransform == null)
        {
            Debug.LogWarning("[RunningCollectibleFeature] Player not found in scene!");
        }
    }

    private void Update()
    {
        if (captured) return;
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (!fleeing && distance <= activationRange && Time.time >= lastFleeTime + fleeCooldown)
        {
            Flee();
        }

        if (distance <= captureRange)
        {
            Capture();
        }
    }

    public void OnInteract(IPuzzleInteractor actor)
    {
        // Optional: allow clicking/tapping to collect immediately (future upgrade)
        Debug.LogWarning("[RunningCollectibleFeature] Capture by proximity, not interaction.");
    }

    private void Flee()
    {
        fleeing = true;
        lastFleeTime = Time.time;

        Vector2 direction = (transform.position - playerTransform.position).normalized;

        // Add random rotation
        float randomAngle = Random.Range(-randomFleeAngleVariance, randomFleeAngleVariance);
        direction = Quaternion.Euler(0, 0, randomAngle) * direction;

        Vector3 targetPosition = transform.position + (Vector3)(direction * fleeDistance);

        if (moveTween != null && moveTween.IsActive())
            moveTween.Kill();

        moveTween = transform.DOMove(targetPosition, fleeDistance / fleeSpeed)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                fleeing = false;
            });
    }

    private void Capture()
    {
        captured = true;

        Debug.Log("[RunningCollectibleFeature] Captured!");

        if (moveTween != null && moveTween.IsActive())
            moveTween.Kill();

        if (collectibleVisual != null)
        {
            Destroy(collectibleVisual);
        }

        RunFeatureEffects();

        Destroy(gameObject, 0.5f);
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