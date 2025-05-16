using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Exit/After Emotion Change")]
public class ExitAfterEmotionChangeSO : ExitStrategySO
{
    [SerializeField] private EmotionTag targetEmotion = EmotionTag.Neutral;
    [SerializeField] private bool requireSpecific = false;
    [SerializeField] private bool invert = false;

    private EmotionTag initialEmotion;
    private bool initialized = false;

    public override bool ShouldExit(IPuzzleInteractor actor, IWorldInteractable target)
    {
        if (EmotionSwitcher.Instance == null) return false;

        if (!initialized)
        {
            initialEmotion = EmotionSwitcher.Instance.GetCurrentEmotion();
            initialized = true;
        }

        EmotionTag current = EmotionSwitcher.Instance.GetCurrentEmotion();
        bool changed = current != initialEmotion;

        if (requireSpecific)
            changed = current == targetEmotion;

        return invert ? !changed : changed;
    }

    public override void OnExit(IPuzzleInteractor companion, IWorldInteractable target)
    {
        initialized = false;
    }
}
