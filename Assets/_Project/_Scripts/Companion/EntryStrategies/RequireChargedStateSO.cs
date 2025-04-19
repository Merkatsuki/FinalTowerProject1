using UnityEngine;

[CreateAssetMenu(menuName = "EntryStrategy/Require Charged State")]
public class RequireChargedStateSO : EntryStrategySO
{
    public override bool CanEnter(CompanionController companion, CompanionClueInteractable target)
    {
        return companion.GetEnergyType() != EnergyType.None;
    }
}



