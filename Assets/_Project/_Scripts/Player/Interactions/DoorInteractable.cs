using UnityEngine;

public class DoorInteractable : InteractableBase
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private float closeDelay = 2.0f;
    private bool isOpen = false;

    public override void OnInteract(IPuzzleInteractor actor)
    {
        if (!isOpen)
        {
            isOpen = true;
            doorAnimator?.SetTrigger("Open");
            var puzzleObj = GetComponent<PuzzleObject>();
            PuzzleInteractionRouter.HandleInteraction(puzzleObj, actor);
            Invoke(nameof(CloseDoor), closeDelay);
        }
    }

    private void CloseDoor()
    {
        isOpen = false;
        doorAnimator?.SetTrigger("Close");
    }
}