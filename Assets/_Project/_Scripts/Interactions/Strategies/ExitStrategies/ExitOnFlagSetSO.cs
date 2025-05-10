using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Exit/ExitOnFlagSet")]
public class ExitOnFlagSetSO : ExitStrategySO
{
    [SerializeField] private FlagSO requiredFlag;

    public override bool ShouldExit(IPuzzleInteractor actor, IWorldInteractable interactable)
    {
        return FlagManager.Instance != null && FlagManager.Instance.IsFlagSet(requiredFlag);

    }

    public void SetRequiredFlag(FlagSO flag)
    {
        requiredFlag = flag;
    }

}