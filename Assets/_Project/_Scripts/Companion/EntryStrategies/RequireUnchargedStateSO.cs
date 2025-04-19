using UnityEngine;

[CreateAssetMenu(menuName = "EntryStrategy/Require Uncharged State")]
public class RequireUnchargedStateSO : EntryStrategySO
{
    public override bool CanEnter(CompanionController companion, CompanionClueInteractable target)
    {
        return companion.GetEnergyType() == EnergyType.None;
    }
}

