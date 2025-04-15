using UnityEngine;

[CreateAssetMenu(menuName = "RobotInteraction/Dock At Zone")]
public class DockAtZoneInteraction : RobotInteractionSO
{
    public override void Execute(CompanionController companion, InteractableBase target)
    {
        DockingZone zone = target.GetComponent<DockingZone>();
        if (zone != null)
        {
            zone.Dock(companion);
        }
    }
}