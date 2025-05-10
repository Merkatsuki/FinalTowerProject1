using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Trigger Quip")]
public class TriggerQuipEffectSO : EffectStrategySO
{
    [Header("Quip Trigger")]
    [SerializeField] private QuipTriggerType triggerType = QuipTriggerType.AmbientRandom;

    protected override void ApplyEffectInternal(IPuzzleInteractor actor, IWorldInteractable target, InteractionResult result)
    {
        if (QuipManager.Instance == null) return;

        QuipManager.Instance.TryPlayQuip(triggerType);
        Debug.Log($"[Effect] Triggered quip: {triggerType} by TriggerQuipEffectSO");
    }
}