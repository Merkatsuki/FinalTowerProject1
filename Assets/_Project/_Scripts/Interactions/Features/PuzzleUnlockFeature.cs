using UnityEngine;
using UnityEngine.Events;

public class PuzzleUnlockFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Puzzle Unlock Settings")]
    [SerializeField] private bool isLocked = true;
    public UnityEvent onUnlocked;

    public void OnInteract(IPuzzleInteractor actor)
    {
        AttemptUnlock();
    }

    public void AttemptUnlock()
    {
        if (isLocked)
        {
            Unlock();
        }
        else
        {
            Debug.Log("[PuzzleUnlockFeature] Already unlocked.");
        }
    }

    public void Unlock()
    {
        isLocked = false;
        Debug.Log("[PuzzleUnlockFeature] Puzzle unlocked!");
        onUnlocked?.Invoke();
    }

    public bool IsUnlocked() => !isLocked;
}
