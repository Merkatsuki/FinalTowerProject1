using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Toggle Wind Zone")]
public class ToggleWindZoneEffect : EffectStrategySO
{
    public enum ToggleMode { EnableDisable, Rotate90 }

    [SerializeField] private ToggleMode mode = ToggleMode.EnableDisable;

    protected override void ApplyEffectInternal(IPuzzleInteractor actor, IWorldInteractable interactable, InteractionResult result)
    {
        var wind = interactable.GetTransform().GetComponentInChildren<WindZone2D>();
        if (wind == null) return;

        switch (mode)
        {
            case ToggleMode.EnableDisable:
                wind.ToggleActive();
                break;

            case ToggleMode.Rotate90:
                var current = wind.windDirection;
                wind.windDirection = new Vector2(-current.y, current.x); // Rotate 90 degrees
                wind.ApplyRotationVisual();
                if (wind.useDirectionalParticles)
                    wind.UpdateActiveParticleSystem(); 
                break;
        }
    }
}

