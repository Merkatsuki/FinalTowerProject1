using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PuzzleLightNode : InteractableBase
{
    [Header("Visuals")]
    [SerializeField] private Light2D lightVisual;
    [SerializeField] private GameObject highlightVisual;

    private int nodeIndex;
    private PuzzleLightController controller;
    public int CurrentColorIndex { get; private set; } = 0;

    public void Initialize(int index, PuzzleLightController controllerRef)
    {
        nodeIndex = index;
        controller = controllerRef;
    }

    public void SetColor(int index)
    {
        CurrentColorIndex = index;
        lightVisual.color = controller.GetColor(index);
    }

    public void SetColor(Color customColor)
    {
        lightVisual.color = customColor;
    }

    public override void OnInteract()
    {
        int nextColor = (CurrentColorIndex + 1) % controller.ColorCycleLength;
        SetColor(nextColor);
    }

    public override void OnFocusEnter() => SetHighlighted(true);
    public override void OnFocusExit() => SetHighlighted(false);
    public override void SetHighlighted(bool isHighlighted)
    {
        if (highlightVisual != null)
            highlightVisual.SetActive(isHighlighted);
    }

    
}
