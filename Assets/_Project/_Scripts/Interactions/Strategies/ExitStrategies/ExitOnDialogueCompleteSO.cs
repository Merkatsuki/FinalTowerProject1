using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Exit/Wait For Dialogue")]
public class ExitOnDialogueCompleteSO : ExitStrategySO
{
    private bool hasTriggeredDialogue = false;

    public override void OnEnter(IPuzzleInteractor actor, IWorldInteractable target)
    {
        if (DialogueManager.Instance == null) return;

        if (target is IDialogueProvider provider)
        {
            DialogueSequence sequence = provider.GetDialogueSequence();
            if (sequence != null && sequence.lines != null && sequence.lines.Count > 0)
            {
                DialogueManager.Instance.ShowDialogue(sequence);
            }
            else
            {
                DialogueManager.Instance.ShowMessage(provider.GetDialogueLine());
            }

            hasTriggeredDialogue = true;
        }
    }

    public override bool ShouldExit(IPuzzleInteractor actor, IWorldInteractable target)
    {
        return hasTriggeredDialogue && !DialogueManager.Instance.IsDialoguePlaying();
    }
}
