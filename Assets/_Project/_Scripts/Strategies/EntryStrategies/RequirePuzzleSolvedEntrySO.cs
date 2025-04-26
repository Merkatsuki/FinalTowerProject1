using UnityEngine;

[CreateAssetMenu(menuName = "Interactions/EntryStrategies/RequirePuzzleSolvedEntry")]
public class RequirePuzzleSolvedEntrySO : EntryStrategySO
{
    [SerializeField] private string requiredPuzzleFlag;

    public override bool CanEnter(IPuzzleInteractor actor, IWorldInteractable interactable)
    {
        if (string.IsNullOrEmpty(requiredPuzzleFlag))
            return true; // If no flag required, allow by default

        return PuzzleManager.Instance != null && PuzzleManager.Instance.IsFlagSet(requiredPuzzleFlag);
    }
}

