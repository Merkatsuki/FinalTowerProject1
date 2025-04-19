using UnityEngine;

[CreateAssetMenu(menuName = "EntryStrategy/Require Matching Energy Type")]
public class RequireMatchingEnergyTypeSO : EntryStrategySO
{
    public override bool CanEnter(CompanionController companion, CompanionClueInteractable target)
    {
        if (!target.TryGetComponent(out EnergyPuzzleGate gate)) return true;

        return companion.GetEnergyType() == gate.GetRequiredEnergy();
    }
}
