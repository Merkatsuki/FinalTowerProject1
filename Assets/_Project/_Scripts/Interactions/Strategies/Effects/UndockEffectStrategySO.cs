using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Companion Undock Effect")]
public class UndockEffectStrategySO : EffectStrategySO
{
    protected override void ApplyEffectInternal(IPuzzleInteractor actor, IWorldInteractable interactable, InteractionResult result)
    {
        var companion = ReferenceManager.Instance?.Companion;

        if (companion == null)
        {
            Debug.LogWarning("[UndockEffect] No CompanionController found in ReferenceManager.");
            return;
        }

        Debug.Log("[UndockEffect] Triggering return to default state.");
        companion.GetFSM().ResumeDefault(companion);
    }
}
