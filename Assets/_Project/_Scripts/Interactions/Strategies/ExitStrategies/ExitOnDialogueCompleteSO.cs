using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Exit/Wait For Dialogue")]
public class ExitOnDialogueCompleteSO : ExitStrategySO
{
    public override void OnEnter(IPuzzleInteractor actor, IWorldInteractable target)
    {
        // No longer needed to trigger dialogue manually here.
        // DialogueFeature or similar should have already started dialogue.
    }

    public override bool ShouldExit(IPuzzleInteractor actor, IWorldInteractable target)
    {
        if (DialogueManager.Instance == null) return true;

        return !DialogueManager.Instance.IsDialoguePlaying();
    }
}
