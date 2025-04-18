// ExitStrategySO.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Exit Strategy/After Dialogue Complete")]
public class ExitAfterDialogueCompleteSO : ExitStrategySO
{
    public override bool ShouldExit(CompanionController companion, InteractableBase target)
    {
        return DialogueManager.Instance != null && !DialogueManager.Instance.IsDialoguePlaying();
    }
}

