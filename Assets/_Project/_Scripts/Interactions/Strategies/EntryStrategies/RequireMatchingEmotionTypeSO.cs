using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Entry/Require Matching Emotion")]
public class RequireMatchingEmotionTypeSO : EntryStrategySO
{
    [SerializeField] private EmotionTag requiredEmotion;

    public override bool CanEnter(IPuzzleInteractor actor, IWorldInteractable target)
    {
        if (EmotionSwitcher.Instance == null) return false;
        return EmotionSwitcher.Instance.GetCurrentEmotion() == requiredEmotion;
    }

    public void SetRequiredEmotionType(EmotionTag tag)
    {
        requiredEmotion = tag;
    }
}
