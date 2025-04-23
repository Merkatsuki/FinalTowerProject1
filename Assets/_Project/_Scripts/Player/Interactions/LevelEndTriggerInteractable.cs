using UnityEngine;

public class LevelEndTriggerInteractable : InteractableBase
{
    public override void OnInteract(IPuzzleInteractor actor)
    {
        Debug.Log($"{actor.GetDisplayName()} triggered end of level.");

        // TODO: Add level transition logic or event
    }
}