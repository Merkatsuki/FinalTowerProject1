using UnityEngine;

public class CollectibleFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Collectible Settings")]
    [SerializeField] private string itemId;
    [SerializeField] private ParticleSystem collectParticles;
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private AudioSource audioSource;

    public void OnInteract(IPuzzleInteractor actor)
    {
        Collect();
    }

    private void Collect()
    {
        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogWarning("[CollectibleFeature] No Item ID set.");
            return;
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(itemId);
            Debug.Log($"[CollectibleFeature] Collected item: {itemId}");
        }
        else
        {
            Debug.LogWarning("[CollectibleFeature] No InventoryManager instance found.");
        }

        PlayCollectEffects();
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
}
