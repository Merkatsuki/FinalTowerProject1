using UnityEngine;

public class PuzzleUnlockInteractable : InteractableBase
{
    [SerializeField] private bool isLocked = true;

    public override bool CanBeInteractedWith(IPuzzleInteractor actor)
    {
        return !isLocked;
    }

    public override void OnInteract(IPuzzleInteractor actor)
    {
        if (!CanBeInteractedWith(actor))
        {
            Debug.Log("Unlock is disabled.");
            return;
        }

        Debug.Log("PuzzleUnlockInteractable activated.");
        var puzzleObj = GetComponent<PuzzleObject>();
        PuzzleInteractionRouter.HandleInteraction(puzzleObj, actor);

        // Additional visual/audio effects can go here
    }
}