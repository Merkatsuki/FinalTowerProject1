using UnityEngine;

public class LevelEndTriggerInteractable : InteractableBase
{
    [SerializeField] private GameObject highlightVisual;
    [SerializeField] private string memoryTag;

    public override void OnFocusEnter() => SetHighlighted(true);
    public override void OnFocusExit() => SetHighlighted(false);

    public override void SetHighlighted(bool isHighlighted)
    {
        if (highlightVisual != null)
            highlightVisual.SetActive(isHighlighted);
    }

    public override void OnInteract()
    {
        MemoryProgressTracker.Instance?.MarkMemoryComplete(memoryTag);
    }
}