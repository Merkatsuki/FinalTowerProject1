using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightToggleInteractable : InteractableBase
{
    [SerializeField] private Light2D targetLight;

    public override void OnInteract(IPuzzleInteractor actor)
    {
        if (targetLight == null) return;

        targetLight.enabled = !targetLight.enabled;
    }
}