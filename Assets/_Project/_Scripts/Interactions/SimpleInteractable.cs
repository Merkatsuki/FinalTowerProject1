using UnityEngine;

public class SimpleInteractable : InteractableBase
{
    [SerializeField] private GameObject highlightVisual;

    public override void OnFocusEnter()
    {
        SetHighlighted(true);
    }

    public override void OnFocusExit()
    {
        SetHighlighted(false);
    }

    public override void SetHighlighted(bool isHighlighted)
    {
        if (highlightVisual != null)
            highlightVisual.SetActive(isHighlighted);
    }

    public override void OnInteract()
    {
        Debug.Log("Interacted with: " + gameObject.name);
    }
}
