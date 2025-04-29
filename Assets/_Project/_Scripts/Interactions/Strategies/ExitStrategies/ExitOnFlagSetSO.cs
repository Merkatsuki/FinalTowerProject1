using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Exit/ExitOnFlagSet")]
public class ExitOnFlagSetSO : ExitStrategySO
{
    [SerializeField] private string requiredFlag;

    public override bool ShouldExit(IPuzzleInteractor actor, IWorldInteractable interactable)
    {
        if (string.IsNullOrEmpty(requiredFlag))
            return false;

        return PuzzleManager.Instance != null && PuzzleManager.Instance.IsFlagSet(requiredFlag);
    }

    public void SetRequiredFlag(string flagName)
    {
        requiredFlag = flagName;
    }

}