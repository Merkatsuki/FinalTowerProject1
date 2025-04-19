using UnityEngine;

[CreateAssetMenu(menuName = "Companion/EntryStrategies/Require Emotion Match")]
public class RequireEmotionMatchSO : EntryStrategySO
{
    [SerializeField] private EmotionType requiredEmotion;

    public override bool CanEnter(CompanionController companion, CompanionClueInteractable target)
    {
        return companion.GetEmotion() == requiredEmotion;
    }
}