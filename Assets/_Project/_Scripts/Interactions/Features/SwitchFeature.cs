using UnityEngine;
using System.Collections.Generic;

public class SwitchFeature : FeatureBase
{
    [Header("Switch Settings")]
    [SerializeField] private bool toggleable = true;
    [SerializeField] private bool startOn = false;
    [SerializeField] private bool oneTimeUse = false;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite spriteOn;
    [SerializeField] private Sprite spriteOff;

    [Header("Wind Zone Actions")]
    [SerializeField] private List<WindZoneAction> windZoneActions = new(); // 👈 new struct list

    private bool isActivated;
    private bool permanentlyUsed;

    protected override void Awake()
    {
        base.Awake();
        isActivated = startOn;
        UpdateVisual();
    }

    public override void OnInteract(IPuzzleInteractor actor)
    {
        if (permanentlyUsed) return;
        if (!toggleable && isActivated) return;

        isActivated = toggleable ? !isActivated : true;

        UpdateVisual();
        isSolved = true;
        NotifyPuzzleInteractionSuccess();

        if (oneTimeUse) permanentlyUsed = true;

        foreach (var entry in windZoneActions)
        {
            if (entry.windZone == null) continue;

            switch (entry.action)
            {
                case WindZoneAction.ActionType.Toggle:
                    Debug.Log($"[SwitchFeature] Toggling WindZone2D: {entry.windZone.name}");
                    entry.windZone.ToggleActive();
                    entry.windZone.PulseLight();
                    break;

                case WindZoneAction.ActionType.Rotate90:
                    Vector2 current = entry.windZone.windDirection;
                    entry.windZone.windDirection = new Vector2(-current.y, current.x);
                    entry.windZone.ApplyRotationVisual();
                    if (entry.windZone.useDirectionalParticles)
                        entry.windZone.UpdateActiveParticleSystem();
                    entry.windZone.PulseLight();
                    Debug.Log($"[SwitchFeature] Rotated WindZone2D: {entry.windZone.name}");
                    break;
            }
        }
    }

    private void UpdateVisual()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = isActivated ? spriteOn : spriteOff;
        }
    }

    public override void ResetPuzzleComponent()
    {
        base.ResetPuzzleComponent();
        isActivated = startOn;
        permanentlyUsed = false;
        UpdateVisual();
    }
}
