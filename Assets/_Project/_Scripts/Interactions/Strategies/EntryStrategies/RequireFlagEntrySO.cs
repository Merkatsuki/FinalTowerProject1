using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Entry/Require Flag Entry")]
public class RequireFlagEntrySO : EntryStrategySO
{
    [SerializeField] private FlagSO targetFlag;
    [SerializeField] private bool requireSet = true;

    public override bool CanEnter(IPuzzleInteractor interactor, IWorldInteractable interactable)
    {
        return targetFlag != null && FlagManager.Instance.GetBool(targetFlag) == requireSet;
    }

}