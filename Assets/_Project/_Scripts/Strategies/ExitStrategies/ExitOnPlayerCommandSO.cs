using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Exit/On Player Command")]
public class ExitOnPlayerCommandSO : ExitStrategySO
{
    [SerializeField] private string commandFlag;

    public override bool ShouldExit(IPuzzleInteractor actor, IWorldInteractable target)
    {
        return PuzzleManager.Instance.IsFlagSet(commandFlag);
    }
}