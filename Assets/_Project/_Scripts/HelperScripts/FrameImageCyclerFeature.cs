using UnityEngine;
using UnityEngine.UI;

public class FrameImageCycleFeature : FeatureBase
{
    [SerializeField] private Sprite[] imageOptions;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SadnessPuzzleRoom owningRoom;

    private int currentIndex = 0;

    public override void OnInteract(IPuzzleInteractor actor)
    {
        CycleNext();
        RunFeatureEffects(); // Triggers effects like sound or particles
        owningRoom?.CheckPuzzleState();
    }

    private void CycleNext()
    {
        if (imageOptions == null || imageOptions.Length == 0 || spriteRenderer == null)
            return;

        currentIndex = (currentIndex + 1) % imageOptions.Length;
        spriteRenderer.sprite = imageOptions[currentIndex];
    }

    public int GetCurrentIndex() => currentIndex;
}