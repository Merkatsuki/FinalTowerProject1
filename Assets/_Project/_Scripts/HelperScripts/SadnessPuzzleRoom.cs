using System.Linq;
using UnityEngine;

public class SadnessPuzzleRoom : MonoBehaviour
{
    [Header("Puzzle Elements")]
    [SerializeField] private PortalLink[] portalLinks;
    [SerializeField] private TeleportFeature[] attachedTeleportFeatures;
    [SerializeField] private FrameImageCycleFeature frameCycler;
    [SerializeField] private PhraseDisplay phraseDisplay;
    [SerializeField] private GameObject vineObject;

    [Header("Puzzle Target")]
    [SerializeField] private string correctPhrase;
    [SerializeField] private int correctImageIndex;

    private bool isSolved;

    public string GetAssignedPhrase() => correctPhrase;
    public int GetCurrentImageIndex() => frameCycler.GetCurrentIndex();

    private void Start()
    {
        vineObject.SetActive(false);
        phraseDisplay.DisplayText(correctPhrase); // Show text on entry
    }

    public void CheckPuzzleState()
    {
        if (isSolved)
            return;

        if (IsCorrectPairing())
        {
            isSolved = true;
            vineObject.SetActive(true);
            Debug.Log($"[SadnessPuzzleRoom] Room {name} solved with correct pairing.");
        }

        SadnessPuzzleRoomManager.Instance.CheckGlobalPuzzleState();

    }

    private bool IsCorrectPairing()
    {
        return frameCycler.GetCurrentIndex() == correctImageIndex &&
               phraseDisplay.GetCurrentPhrase() == correctPhrase;
    }

    public PortalLink GetLink(PortalSide fromSide)
    {
        var link = portalLinks.FirstOrDefault(p => p.sourceSide == fromSide);
        if (link == null)
        {
            Debug.LogWarning($"[PortalLink] No PortalLink found for side {fromSide} in room {name}");
        }
        else
        {
            Debug.Log($"[PortalLink] From {name}.{fromSide} → {link.targetRoom.name}.{link.targetSide}");
        }
        return link;
    }

    public TeleportFeature GetFeatureForSide(PortalSide side)
    {
        var feature = attachedTeleportFeatures.FirstOrDefault(f => f.GetSide() == side);
        if (feature == null)
        {
            Debug.LogWarning($"[TeleportFeature] Could not find feature for {side} in {name}");
        }
        else
        {
            Debug.Log($"[TeleportFeature] Found destination feature in {name}.{side}");
        }
        return feature;
    }

    public void RefreshState()
    {
        // Re-show current image, phrase, vine, etc. if needed
        Debug.Log($"[SadnessPuzzleRoom] Refreshing visual state for room: {name}");
    }

    public bool IsSolved() => isSolved;
}
