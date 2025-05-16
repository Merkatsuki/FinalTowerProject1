using System.Collections.Generic;
using UnityEngine;

public enum FlagCheckMode
{
    Single,
    All,
    Any
}

[CreateAssetMenu(menuName = "Strategies/Exit/Exit On Flag Set")]
public class ExitOnFlagSetSO : ExitStrategySO
{
    [Header("Single Flag")]
    [SerializeField] private FlagSO requiredFlag;
    [SerializeField] private bool requireSet = true;

    [Header("Multi-Flag Check")]
    [SerializeField] private FlagCheckMode checkMode = FlagCheckMode.Single;
    [SerializeField] private List<FlagSO> flagList = new();

    public override bool ShouldExit(IPuzzleInteractor actor, IWorldInteractable interactable)
    {
        if (FlagManager.Instance == null)
            return false;

        switch (checkMode)
        {
            case FlagCheckMode.Single:
                if (requiredFlag == null) return false;
                return requireSet
                    ? FlagManager.Instance.IsFlagSet(requiredFlag)
                    : !FlagManager.Instance.IsFlagSet(requiredFlag);

            case FlagCheckMode.All:
                foreach (var flag in flagList)
                {
                    if (flag == null) continue;
                    if (requireSet && !FlagManager.Instance.IsFlagSet(flag))
                        return false;
                    if (!requireSet && FlagManager.Instance.IsFlagSet(flag))
                        return false;
                }
                return true;

            case FlagCheckMode.Any:
                foreach (var flag in flagList)
                {
                    if (flag == null) continue;
                    if (requireSet && FlagManager.Instance.IsFlagSet(flag))
                        return true;
                    if (!requireSet && !FlagManager.Instance.IsFlagSet(flag))
                        return true;
                }
                return false;

            default:
                return false;
        }
    }
}
