using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Momentum;

public class PortalFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Teleport Settings")]
    [SerializeField] private bool teleportToScene = false;
    [SerializeField] private string targetSceneName;
    [SerializeField] private Transform localTeleportTarget;
    [SerializeField] private float teleportDelay = 0f;

    [Header("Visuals & Effects")]
    [SerializeField] private bool useFadeTransition = true;
    [SerializeField] private GameObject portalVisualEffectPrefab;
    [SerializeField] private List<EffectStrategySO> featureEffects = new();

    private bool teleporting = false;
    private Player player;

    private void Awake()
    {
        // Find player at runtime (safe decoupled)
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogWarning("[PortalFeature] No Player found in scene!");
        }
    }

    public void OnInteract(IPuzzleInteractor actor)
    {
        if (teleporting) return;
        teleporting = true;

        if (useFadeTransition && ScreenFader.Instance != null)
        {
            ScreenFader.Instance.FadeOut(() =>
            {
                StartCoroutine(DoTeleport());
            });
        }
        else
        {
            StartCoroutine(DoTeleport());
        }
    }

    private IEnumerator DoTeleport()
    {
        if (teleportDelay > 0f)
        {
            yield return new WaitForSeconds(teleportDelay);
        }

        if (portalVisualEffectPrefab != null)
        {
            Instantiate(portalVisualEffectPrefab, transform.position, Quaternion.identity);
        }

        if (teleportToScene && !string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else if (localTeleportTarget != null)
        {
            if (player != null)
            {
                player.transform.position = localTeleportTarget.position;
            }
            else
            {
                Debug.LogWarning("[PortalFeature] Player reference missing during teleport!");
            }
        }
        else
        {
            Debug.LogWarning("[PortalFeature] No teleport target set!");
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
