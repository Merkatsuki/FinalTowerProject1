using UnityEngine;

[CreateAssetMenu(menuName = "EntryStrategy/Require Energy Gate Not Activated")]
public class RequireEnergyGateNotActivatedSO : EntryStrategySO
{
    public override bool CanEnter(CompanionController companion, CompanionClueInteractable target)
    {
        if (target.TryGetComponent(out EnergyPuzzleGate gate))
        {
            return !gate.IsActivated();
        }
        return true;
    }
}

