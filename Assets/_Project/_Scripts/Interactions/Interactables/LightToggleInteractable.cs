using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightToggleInteractable : InteractableBase
{
    [SerializeField] private GameObject highlightVisual;
    [SerializeField] private Light2D lightToToggle;
    [SerializeField] private InteractableBase[] linkedInteractables;

    private void Start()
    {
        if (lightToToggle != null)
        {
            ToggleLinkedInteractables(lightToToggle.enabled);
        }
    }

    public override void OnFocusEnter() => SetHighlighted(true);
    public override void OnFocusExit() => SetHighlighted(false);

    public override void SetHighlighted(bool isHighlighted)
    {
        if (highlightVisual != null)
            highlightVisual.SetActive(isHighlighted);
    }

    public override void OnInteract()
    {
        if (lightToToggle != null)
        {
            bool newState = !lightToToggle.enabled;
            lightToToggle.enabled = newState;
            Debug.Log($"Light toggled: {newState}");

            ToggleLinkedInteractables(newState);
        }
    }

    private void ToggleLinkedInteractables(bool isVisible)
    {
        foreach (var interactable in linkedInteractables)
        {
            if (interactable == null) continue;

            interactable.gameObject.SetActive(isVisible);
        }
    }

}
