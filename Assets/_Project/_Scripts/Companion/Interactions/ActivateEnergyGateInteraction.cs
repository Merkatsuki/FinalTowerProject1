// ActivateEnergyGateInteraction.cs
using UnityEngine;

[CreateAssetMenu(menuName = "RobotInteraction/Activate Energy Gate")]
public class ActivateEnergyGateInteraction : RobotInteractionSO
{
    public override void Execute(CompanionController companion, CompanionClueInteractable target)
    {
        if (!target.TryGetComponent(out EnergyPuzzleGate gate))
        {
            Debug.LogWarning("ActivateEnergyGateInteraction: Target does not have a PuzzleEnergyGate component.");
            DialogueManager.Instance?.ShowMessage("This doesn't look like an energy gate...");
            return;
        }

        EnergyType energy = companion.GetEnergyType();

        if (energy == gate.GetRequiredEnergy() && !gate.IsActivated())
        {
            gate.AcceptEnergyFrom(companion);
            DialogueManager.Instance?.ShowMessage($"Gate activated with {energy} energy.");
        }
        else
        {
            DialogueManager.Instance?.ShowMessage("Gate requires a different energy type.");
        }
    }
}
