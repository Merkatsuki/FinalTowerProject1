using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Companion Move To Dock")]
public class CompanionDockEffectSO : EffectStrategySO
{
    [SerializeField] private float hoverTime = 1.5f;
    [SerializeField] private bool stayIndefinitely = false;

    protected override void ApplyEffectInternal(IPuzzleInteractor actor, IWorldInteractable interactable, InteractionResult result)
    {
        Debug.Log("[DockEffect] Starting ApplyEffectInternal");
        Debug.Log($"[DockEffect] Actor: {actor}, Interactable: {interactable}");

        //if (actor is not CompanionController companion) return;

        if (interactable is InteractableBase baseInteractable)
        {
            Vector3 dockPosition = baseInteractable.GetDockPosition();

            DockConfig config = new DockConfig(
                dockPosition,
                hoverTime,
                () => baseInteractable.MarkDocked(),
                stayIndefinitely
            );

            Debug.Log($"[DockEffect] Companion docking at position: {dockPosition} with hoverTime {hoverTime}");
            ReferenceManager.Instance.Companion.DockTo(config);
        }
        else
        {
            Debug.LogWarning("[DockEffect] Interactable is not an InteractableBase. Docking aborted.");
        }
    }
}
