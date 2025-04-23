using UnityEngine;

public class LightToggleInteractable : InteractableBase
{
    [SerializeField] private Light targetLight;

    public override void OnInteract(IPuzzleInteractor actor)
    {
        if (targetLight == null) return;

        targetLight.enabled = !targetLight.enabled;
    }
}