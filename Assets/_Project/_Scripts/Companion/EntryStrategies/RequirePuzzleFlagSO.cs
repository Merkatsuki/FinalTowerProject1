using UnityEngine;

[CreateAssetMenu(menuName = "EntryStrategy/Require Puzzle Flag")]
public class RequirePuzzleFlagSO : EntryStrategySO
{
    [SerializeField] private string flagKey;

    public override bool CanEnter(CompanionController companion, CompanionClueInteractable target)
    {
        return PuzzleManager.Instance != null && PuzzleManager.Instance.IsFlagSet(flagKey);
    }
}



