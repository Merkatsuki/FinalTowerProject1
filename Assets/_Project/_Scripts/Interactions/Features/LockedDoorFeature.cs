using UnityEngine;

public class LockedDoorFeature : MonoBehaviour, IInteractableFeature
{
    [SerializeField] private bool isLocked = true;
    [SerializeField] private string requiredFlag;

    private Animator doorAnimator;

    private void Awake()
    {
        doorAnimator = GetComponent<Animator>();
        if (doorAnimator == null)
        {
            doorAnimator = gameObject.AddComponent<Animator>();
            Debug.LogWarning("[LockedDoorFeature] No Animator found. Added default Animator component. Assign a Controller manually!");
        }
    }

    public void OnInteract(IPuzzleInteractor actor)
    {
        AttemptUnlock();
    }

    public bool CanUnlock()
    {
        if (!isLocked)
            return true;

        if (string.IsNullOrEmpty(requiredFlag))
            return false;

        return PuzzleManager.Instance != null && PuzzleManager.Instance.IsFlagSet(requiredFlag);
    }

    public void AttemptUnlock()
    {
        if (CanUnlock())
        {
            Unlock();
        }
        else
        {
            Debug.Log("[LockedDoorFeature] Unlock conditions not met.");
        }
    }

    public void Unlock()
    {
        isLocked = false;
        Debug.Log("[LockedDoorFeature] Door unlocked.");

        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Open");
        }
    }

    public bool IsUnlocked() => !isLocked;
}