using UnityEngine;

[CreateAssetMenu(menuName = "Interactions/ExitStrategies/ExitOnFlagSet")]
public class ExitOnFlagSetSO : ExitStrategySO
{
    [SerializeField] private string requiredFlag;

    public override bool ShouldExit(IPuzzleInteractor actor, IWorldInteractable interactable)
    {
        if (string.IsNullOrEmpty(requiredFlag))
            return false;

        return PuzzleManager.Instance != null && PuzzleManager.Instance.IsFlagSet(requiredFlag);
    }
}