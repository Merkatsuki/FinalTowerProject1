using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Companion Undock")]
public class CompanionUndockEffectSO : EffectStrategySO
{
    protected override void ApplyEffectInternal(IPuzzleInteractor actor, IWorldInteractable interactable, InteractionResult result)
    {
        ReferenceManager.Instance.Companion.fsm.ResumeDefault(ReferenceManager.Instance.Companion);
    }
}