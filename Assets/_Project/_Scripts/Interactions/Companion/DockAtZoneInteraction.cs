using UnityEngine;

[CreateAssetMenu(menuName = "RobotInteraction/Dock At Zone")]
public class DockAtZoneInteraction : RobotInteractionSO
{
    public override void Execute(CompanionController companion, InteractableBase target)
    {
        if (target.TryGetComponent<EnergyDockingZone>(out var zone))
        {
            zone.Dock(companion);
            var energy = companion.GetEnergyType();

            DialogueManager.Instance?.ShowMessage($"Docked at {zone.name}. Charged with {energy} energy.");
        }
        else
        {
            Debug.LogWarning("[DockAtZoneInteraction] Target has no EnergyDockingZone component.");
            DialogueManager.Instance?.ShowMessage("Unable to dock here.");
        }
    }
}