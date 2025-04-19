using UnityEngine;

[CreateAssetMenu(menuName = "RobotInteraction/Scan Clue")]
public class ScanClueInteraction : RobotInteractionSO
{
    [TextArea]
    public string scanMessage = "Analyzing memory fragment...";

    public override void Execute(CompanionController companion, CompanionClueInteractable target)
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowMessage(scanMessage);
        }
        else
        {
            Debug.LogWarning("[ScanClueInteraction] DialogueManager not found in scene.");
        }
    }
}
