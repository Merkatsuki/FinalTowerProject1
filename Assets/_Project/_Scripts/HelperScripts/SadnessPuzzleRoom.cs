using System.Linq;
using UnityEngine;

public class SadnessPuzzleRoom : MonoBehaviour
{
    [Header("Puzzle Elements")]
    [SerializeField] private PortalLink[] portalLinks;
    [SerializeField] private TeleportFeature[] attachedTeleportFeatures;
    [SerializeField] private FrameImageCycleFeature frameCycler;
    [SerializeField] private PhraseDisplay phraseDisplay;
    [SerializeField] private FloatingWordReactiveEffect floatingWordEffect;

    [Header("Puzzle Target")]
    [SerializeField] private string correctPhrase;
    [SerializeField] private int correctImageIndex;

    public bool IsSolved() => IsCorrectPairing();
    public string GetAssignedPhrase() => correctPhrase;
    public int GetCurrentImageIndex() => frameCycler.GetCurrentIndex();

    private void Start()
    {
        phraseDisplay.DisplayText(correctPhrase);

        // Set a random starting index that avoids the correct one
        int total = frameCycler.GetTotalImageCount();
        int startIndex = Random.Range(0, total);

        if (startIndex == correctImageIndex && total > 1)
            startIndex = (startIndex + 1) % total;

        frameCycler.SetImageIndex(startIndex);

        ValidateImageAgainstPhrase(); // Apply word effect status
    }


    public void CheckPuzzleState()
    {
        if (IsCorrectPairing())
        {
        }

        SadnessPuzzleRoomManager.Instance.CheckGlobalPuzzleState();
    }


    public void ValidateImageAgainstPhrase()
    {
        bool correct = IsCorrectPairing();
        floatingWordEffect?.SetActiveEffect(correct);
    }

    private bool IsCorrectPairing()
    {
        return frameCycler.GetCurrentIndex() == correctImageIndex &&
               phraseDisplay.GetCurrentPhrase() == correctPhrase;
    }

    public PortalLink GetLink(PortalSide fromSide)
    {
        var link = portalLinks.FirstOrDefault(p => p.sourceSide == fromSide);
        
        return link;
    }

    public TeleportFeature GetFeatureForSide(PortalSide side)
    {
        var feature = attachedTeleportFeatures.FirstOrDefault(f => f.GetSide() == side);
        
        return feature;
    }

    public void RefreshState()
    {
        // Re-show current image, phrase, vine, etc. if needed
    }
}

