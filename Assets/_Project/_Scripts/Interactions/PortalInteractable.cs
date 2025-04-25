using UnityEngine;

public class PortalInteractable : InteractableBase
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private bool isLocked = true;

    public override bool CanBeInteractedWith(IPuzzleInteractor actor)
    {
        return !isLocked;
    }

    public override void OnInteract(IPuzzleInteractor actor)
    {
        if (!CanBeInteractedWith(actor))
        {
            Debug.Log("Portal is locked.");
            return;
        }

        Debug.Log($"{actor.GetDisplayName()} activated portal to {sceneToLoad}.");

        // TODO: Scene loading logic goes here
    }

    public void Unlock()
    {
        isLocked = false;
        Debug.Log("Portal unlocked.");
    }
}