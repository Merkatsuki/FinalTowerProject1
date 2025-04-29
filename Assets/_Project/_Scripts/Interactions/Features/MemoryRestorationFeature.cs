using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class MemoryRestorationFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Restoration Settings")]
    [SerializeField] private GameObject brokenVisual;
    [SerializeField] private GameObject restoredVisual;
    [SerializeField] private bool playRestoreAnimation = true;
    [SerializeField] private float restoreDuration = 1.5f;
    [SerializeField] private Ease restoreEase = Ease.OutBack;

    [Header("Feature Effects")]
    [SerializeField] private List<EffectStrategySO> featureEffects = new();

    private bool restored = false;

    private void Start()
    {
        if (brokenVisual != null)
            brokenVisual.SetActive(true);

        if (restoredVisual != null)
        {
            restoredVisual.SetActive(false);
            restoredVisual.transform.localScale = Vector3.zero;
        }
    }

    public void OnInteract(IPuzzleInteractor actor)
    {
        if (restored) return;

        RestoreMemory();
    }

    public void RestoreMemory()
    {
        if (restored) return;
        restored = true;

        Debug.Log("[MemoryRestorationFeature] Memory restored!");

        if (brokenVisual != null)
            brokenVisual.SetActive(false);

        if (restoredVisual != null)
        {
            restoredVisual.SetActive(true);

            if (playRestoreAnimation)
            {
                restoredVisual.transform.localScale = Vector3.zero;
                restoredVisual.transform.DOScale(Vector3.one, restoreDuration).SetEase(restoreEase);
            }
            else
            {
                restoredVisual.transform.localScale = Vector3.one;
            }
        }

        RunFeatureEffects();
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

