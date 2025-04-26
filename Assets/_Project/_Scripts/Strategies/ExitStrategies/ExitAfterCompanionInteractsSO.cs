using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Exit/ExitAfterCompanionInteracts")]
public class ExitAfterCompanionInteractsSO : ExitStrategySO
{
    private bool companionInteracted = false;

    // IMPORTANT: Requires CompanionController (or similar) to call MarkCompanionInteracted() manually after interaction complete.

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