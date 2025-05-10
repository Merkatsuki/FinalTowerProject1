using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Switch Emotion Mode")]
public class SwitchEmotionModeEffectSO : EffectStrategySO
{
    [SerializeField] private EmotionTag emotionToSet;

    protected override void ApplyEffectInternal(IPuzzleInteractor actor, IWorldInteractable target, InteractionResult result)
    {
        if (EmotionSwitcher.Instance == null) return;

        EmotionSwitcher.Instance.SetEmotion(emotionToSet);
        Debug.Log($"[Effect] Emotion set to {emotionToSet} by SwitchEmotionModeEffectSO");
    }
}
