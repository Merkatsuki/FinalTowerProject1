using UnityEngine;

public enum PortalMode
{
    LocalTeleport,
    SceneTransition
}

public class PortalFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Portal Settings")]
    [SerializeField] private PortalMode mode = PortalMode.LocalTeleport;
    [SerializeField] private Transform linkedPortal;
    [SerializeField] private string sceneToLoad;
    [SerializeField] private bool isLocked = false;

    [Header("Portal Meta Settings")]
    [SerializeField] private string portalID;

    private void Awake()
    {
        // Ensure tag is set
        if (gameObject.tag != "Portal")
        {
            gameObject.tag = "Portal";
        }

        // Ensure a portal marker exists
        if (transform.Find("PortalMarker") == null)
        {
            GameObject marker = new GameObject("PortalMarker");
            marker.transform.SetParent(transform);
            marker.transform.localPosition = Vector3.zero;
            var sr = marker.AddComponent<SpriteRenderer>();
            sr.sprite = null; // You can assign a default portal sprite in your project later
            sr.sortingOrder = 5;
            Debug.Log("[PortalFeature] Auto-created PortalMarker child.");
        }
    }

    public void OnInteract(IPuzzleInteractor actor)
    {
        ActivatePortal();
    }

    public void ActivatePortal()
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
                    // TODO: Plug in your scene loading logic here
                }
                break;
        }
    }

    public void UnlockPortal()
    {
        isLocked = false;
        Debug.Log("[PortalFeature] Portal unlocked.");
    }

    public bool IsPortalUnlocked() => !isLocked;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (linkedPortal != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, linkedPortal.position);
        }
    }
#endif
}