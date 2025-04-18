using UnityEngine;

public class PuzzleUnlockInteractable : InteractableBase
{
    [SerializeField] private PuzzleLightController puzzleController;
    [SerializeField] private GameObject highlightVisual;

    private bool isLocked = false;

    private void Start()
    {
        if (puzzleController != null)
        {
            puzzleController.OnPuzzleSolved += HandlePuzzleSolved;
        }
    }

    private void OnDestroy()
    {
        if (puzzleController != null)
        {
            puzzleController.OnPuzzleSolved -= HandlePuzzleSolved;
        }
    }

    private void HandlePuzzleSolved()
    {
        isLocked = true;
        if (highlightVisual != null)
            highlightVisual.SetActive(false);

        Debug.Log("Unlock Interactable disabled after puzzle was solved.");
    }

    public override void OnInteract()
    {
        if (isLocked) return;

        if (puzzleController != null)
        {
            puzzleController.TrySolvePuzzle();
        }
        else
        {
            Debug.LogWarning("Unlock interactable missing PuzzleLightController reference.");
        }
    }

    public override void OnFocusEnter()
    {
        if (!isLocked)
            SetHighlighted(true);
    }

    public override void OnFocusExit()
    {
        if (!isLocked)
            SetHighlighted(false);
    }

    public override void SetHighlighted(bool isHighlighted)
    {
        if (highlightVisual != null)
            highlightVisual.SetActive(isHighlighted);
    }

    public override bool CanInteract => !isLocked;
}
