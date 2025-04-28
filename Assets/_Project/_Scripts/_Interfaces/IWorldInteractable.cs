using System.Collections.Generic;
using UnityEngine;

public interface IWorldInteractable
{
    List<ExitStrategySO> GetExitStrategies();
    string GetDisplayName();
    Transform GetTransform();
    bool CanBeInteractedWith(IPuzzleInteractor actor);
    void OnInteract(IPuzzleInteractor actor);
    void OnInteractionComplete(IPuzzleInteractor actor, bool interactionSucceeded);
    void SetHighlight(bool enabled);
    
}
