using UnityEngine;

public class FinalPortalAutoUnlocker : MonoBehaviour
{
    [SerializeField] private PortalInteractable portalToUnlock;

    private void Start()
    {
        if (portalToUnlock == null)
        {
            Debug.LogWarning("FinalPortalAutoUnlocker: No portal assigned to unlock.");
            return;
        }

        if (MemoryProgressTracker.Instance != null &&
            MemoryProgressTracker.Instance.AreAllTrackedMemoriesComplete())
        {
            portalToUnlock.Unlock();
        }
    }
}

