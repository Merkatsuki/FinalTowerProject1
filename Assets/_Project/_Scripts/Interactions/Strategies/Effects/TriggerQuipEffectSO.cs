using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Trigger Quip")]
public class TriggerQuipEffectSO : EffectStrategySO
{
    public bool useCustomQuipText = false;

    [TextArea]
    public string customQuip;

    [Header("Quip Trigger")]
    [SerializeField] private QuipTriggerType triggerType = QuipTriggerType.AmbientRandom;

    protected override void ApplyEffectInternal(IPuzzleInteractor actor, IWorldInteractable target, InteractionResult result)
    {
        if (useCustomQuipText && !string.IsNullOrWhiteSpace(customQuip))
        {
            QuipManager.Instance?.PlayDirectQuip(customQuip);
        }
        else
        {
            QuipManager.Instance?.TryPlayFilteredQuip(triggerType, actor as CompanionController);
        }
    }
}