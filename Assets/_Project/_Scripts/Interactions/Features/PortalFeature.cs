using UnityEngine;
using System.Collections.Generic;

public class PortalFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Portal Settings")]
    [SerializeField] private PortalMode mode = PortalMode.LocalTeleport;
    [SerializeField] private Transform linkedPortal;
    [SerializeField] private string sceneToLoad;
    [SerializeField] private bool isLocked = false;

    [Header("Portal Meta Settings")]
    [SerializeField] private string portalID;

    [Header("Feature Effects")]
    [SerializeField] private List<EffectStrategySO> featureEffects = new();

    private void Awake()
    {
        if (gameObject.tag != "Portal")
        {
            gameObject.tag = "Portal";
        }

        if (transform.Find("PortalMarker") == null)
        {
            GameObject marker = new GameObject("PortalMarker");
            marker.transform.SetParent(transform);
            marker.transform.localPosition = Vector3.zero;
            var sr = marker.AddComponent<SpriteRenderer>();
            sr.sprite = null;
            sr.sortingOrder = 5;
            Debug.Log("[PortalFeature] Auto-created PortalMarker child.");
        }
    }

    public void OnInteract(IPuzzleInteractor actor)
    {
        ActivatePortal(actor);
    }

    public void ActivatePortal(IPuzzleInteractor actor)
    {
        if (isLocked)
        {
            Debug.Log("[PortalFeature] Portal is locked.");
            return;
        }

        switch (mode)
        {
            case PortalMode.LocalTeleport:
                if (linkedPortal != null)
                {
                    Debug.Log("[PortalFeature] Teleporting to linked portal.");
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player != null)
                        player.transform.position = linkedPortal.position;
                }
                break;

            case PortalMode.SceneTransition:
                if (!string.IsNullOrEmpty(sceneToLoad))
                {
                    Debug.Log($"[PortalFeature] Loading scene: {sceneToLoad}.");
                    // TODO: Add scene loading logic here
                }
                break;
        }

        RunFeatureEffects(actor);
    }

    public void UnlockPortal()
    {
        isLocked = false;
        Debug.Log("[PortalFeature] Portal unlocked.");
    }

    public bool IsPortalUnlocked() => !isLocked;

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

    public void SetFeatureEffects(List<EffectStrategySO> effects)
    {
        featureEffects = effects ?? new List<EffectStrategySO>();
    }
}
public enum PortalMode
{
    LocalTeleport,
    SceneTransition
}
