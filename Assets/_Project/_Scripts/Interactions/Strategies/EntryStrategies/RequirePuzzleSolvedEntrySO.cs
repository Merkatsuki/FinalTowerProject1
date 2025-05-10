using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Entry/RequirePuzzleSolvedEntry")]
public class RequirePuzzleSolvedEntrySO : EntryStrategySO
{
    [SerializeField] private FlagSO requiredFlag;


    public override bool CanEnter(IPuzzleInteractor actor, IWorldInteractable interactable)
    {
        return FlagManager.Instance != null && FlagManager.Instance.IsFlagSet(requiredFlag);

    }

    public void SetRequiredPuzzleFlag(FlagSO flag)
    {
        requiredFlag = flag;
    }

}

