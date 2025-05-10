using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Exit/After Emotion Change")]
public class ExitAfterEmotionChangeSO : ExitStrategySO
{
    private EmotionTag initialEmotion;
    private bool initialized = false;

    public override bool ShouldExit(IPuzzleInteractor actor, IWorldInteractable target)
    {
        if (!initialized && EmotionSwitcher.Instance != null)
        {
            initialEmotion = EmotionSwitcher.Instance.GetCurrentEmotion();
            initialized = true;
        }

        EmotionTag current = EmotionSwitcher.Instance?.GetCurrentEmotion() ?? EmotionTag.Neutral;
        return current != initialEmotion;
    }

    public override void OnExit(IPuzzleInteractor companion, IWorldInteractable target)
    {
        initialized = false; // Reset when exiting so it can be reused
    }
}

