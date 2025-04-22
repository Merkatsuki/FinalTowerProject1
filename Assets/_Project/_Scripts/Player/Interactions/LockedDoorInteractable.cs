using UnityEngine;

public class LockedDoorInteractable : InteractableBase
{
    [SerializeField] private bool isLocked = true;
    [SerializeField] private string requiredFlag;

    public override bool CanBeInteractedWith(IPuzzleInteractor actor)
    {
        return !isLocked || PuzzleManager.Instance.IsFlagSet(requiredFlag);
    }

    public override void OnInteract(IPuzzleInteractor actor)
    {
        if (!CanBeInteractedWith(actor))
        {
            Debug.Log("Door is locked.");
            return;
        }

        isLocked = false;
        Debug.Log("Locked door unlocked.");
        var puzzleObj = GetComponent<PuzzleObject>();
        PuzzleInteractionRouter.HandleInteraction(puzzleObj, actor);
    }
}