using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FrameImageCycleFeature : FeatureBase
{
    [SerializeField] private Sprite[] imageOptions;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SadnessPuzzleRoom owningRoom;
    [SerializeField] private TextMeshProUGUI indexLabel;

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
        UpdateIndexLabel();
        owningRoom?.ValidateImageAgainstPhrase();
    }

    public void SetImageIndex(int index)
    {
        if (imageOptions == null || imageOptions.Length == 0)
            return;

        currentIndex = Mathf.Clamp(index, 0, imageOptions.Length - 1);
        spriteRenderer.sprite = imageOptions[currentIndex];
        UpdateIndexLabel();
    }

    private void UpdateIndexLabel()
    {
        if (indexLabel != null)
            indexLabel.text = (currentIndex + 1).ToString();
    }

    public int GetTotalImageCount() => imageOptions?.Length ?? 0;

    public int GetCurrentIndex() => currentIndex;
}
