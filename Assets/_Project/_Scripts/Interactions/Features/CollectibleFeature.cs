using UnityEngine;
using System.Collections.Generic;

public class CollectibleFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Collectible Settings")]
    [SerializeField] private ParticleSystem collectParticles;
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private AudioSource audioSource;

    [Header("Feature Effects")]
    [SerializeField] private List<EffectStrategySO> featureEffects = new();

    public void OnInteract(IPuzzleInteractor actor)
    {
        Collect(actor);
    }

    private void Collect(IPuzzleInteractor actor)
    {
        PlayCollectEffects();
        RunFeatureEffects(actor);
        Destroy(gameObject);
    }

    private void PlayCollectEffects()
    {
        if (collectParticles != null)
        {
            Instantiate(collectParticles, transform.position, Quaternion.identity);
        }

        if (collectSound != null)
        {
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.PlayOneShot(collectSound);
        }
    }

    private void RunFeatureEffects(IPuzzleInteractor actor)
    {
        foreach (var effect in featureEffects)
        {
            if (effect != null)
            {
                if (TryGetComponent(out IWorldInteractable interactable))
                {
                    effect.ApplyEffect(actor, interactable, InteractionResult.Success);
                }
                else
                {
                    Debug.LogWarning("[CollectibleFeature] No IWorldInteractable found for effect execution.");
                }
            }
        }
    }

    public void SetFeatureEffects(List<EffectStrategySO> effects)
    {
        featureEffects = effects ?? new List<EffectStrategySO>();
    }
}
