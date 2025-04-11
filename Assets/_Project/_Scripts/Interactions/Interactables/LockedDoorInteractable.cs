using UnityEngine;

public class LockedDoorInteractable : DoorInteractable, IActivatable
{
    [SerializeField] private GameObject lockedIcon;

    [Header("Unlock Requirements")]
    [SerializeField] private bool requiresKey = false;
    [SerializeField] private string requiredItemID;

    [SerializeField] private bool requiresPuzzle = false;

    private bool keyUnlocked = false;
    private bool puzzleUnlocked = false;
    private bool isFullyUnlocked = false;

    public override void OnInteract()
    {
        CheckUnlockStatus();

        if (!isFullyUnlocked)
        {
            if (requiresKey && !keyUnlocked)
                Debug.Log("Door is locked. A key is required.");

            if (requiresPuzzle && !puzzleUnlocked)
                Debug.Log("Door is locked. A puzzle needs to be solved.");

            return;
        }

        base.OnInteract(); // Open
    }

    public void Activate()
    {
        // Called by a puzzle light when puzzle is solved
        if (requiresPuzzle && !puzzleUnlocked)
        {
            puzzleUnlocked = true;
            Debug.Log("Door: puzzle condition fulfilled.");
            CheckUnlockStatus();
        }
    }

    private void CheckUnlockStatus()
    {
        // Key check happens once on interact
        if (requiresKey && !keyUnlocked && InventoryManager.Instance != null)
        {
            if (InventoryManager.Instance.HasItem(requiredItemID))
            {
                keyUnlocked = true;
                Debug.Log("Door: key requirement fulfilled.");
            }
        }

        // Final check using only flags
        if ((!requiresKey || keyUnlocked) && (!requiresPuzzle || puzzleUnlocked))
        {
            if (!isFullyUnlocked)
            {
                isFullyUnlocked = true;
                Debug.Log("Door is now fully unlocked!");

                if (lockedIcon != null)
                {
                    lockedIcon.SetActive(false);
                }
            }
        }
    }
}
