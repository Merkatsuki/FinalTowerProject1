using UnityEngine;

[CreateAssetMenu(menuName = "RobotInteraction/Scan Clue")]
public class ScanClueInteraction : RobotInteractionSO
{
    public string scanMessage = "Analyzing memory fragment...";

    public override void Execute(CompanionController companion, InteractableBase target)
    {
        Debug.Log($"[Robot Scan]: {target.name} -> {scanMessage}");
        // TODO: trigger VFX, SFX, update game state, etc.
    }
}
