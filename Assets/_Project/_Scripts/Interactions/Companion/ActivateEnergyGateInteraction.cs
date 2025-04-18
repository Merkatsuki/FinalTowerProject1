using UnityEngine;

[CreateAssetMenu(menuName = "RobotInteraction/Activate Energy Gate")]
public class ActivateEnergyGateInteraction : RobotInteractionSO
{
    public override void Execute(CompanionController companion, InteractableBase target)
    {
        if (target.TryGetComponent<EnergyPuzzleGate>(out var gate))
        {
            var robotEnergy = companion.GetEnergyType();
            var requiredEnergy = gate.requiredEnergy;

            if (robotEnergy == requiredEnergy)
            {
                gate.TryActivate(companion);
                DialogueManager.Instance?.ShowMessage($"Energy match successful: {robotEnergy}. Gate unlocking...");
            }
            else
            {
                DialogueManager.Instance?.ShowMessage($"Energy mismatch. Required: {requiredEnergy}. Current: {robotEnergy}.");
            }
        }
        else
        {
            Debug.LogWarning("[ActivateEnergyGateInteraction] Target does not contain an EnergyPuzzleGate component.");
        }
    }
}