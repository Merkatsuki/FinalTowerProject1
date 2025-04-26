using UnityEngine;

[CreateAssetMenu(menuName = "Interactions/ExitStrategies/ExitAfterCompanionInteracts")]
public class ExitAfterCompanionInteractsSO : ExitStrategySO
{
    private bool companionInteracted = false;

    public override bool ShouldExit(IPuzzleInteractor actor, IWorldInteractable interactable)
    {
        return companionInteracted;
    }

    public void MarkCompanionInteracted()
    {
        companionInteracted = true;
    }

    public override void OnEnter(IPuzzleInteractor actor, IWorldInteractable interactable)
    {
        companionInteracted = false;
    }
}