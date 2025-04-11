using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightToggleInteractable : InteractableBase
{
    [SerializeField] private GameObject highlightVisual;
    [SerializeField] private Light2D lightToToggle;

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
            lightToToggle.enabled = !lightToToggle.enabled;
            Debug.Log($"Light toggled: {lightToToggle.enabled}");
        }
    }
}