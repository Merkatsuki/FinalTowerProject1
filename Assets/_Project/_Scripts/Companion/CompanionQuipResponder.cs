using UnityEngine;

public class CompanionQuipResponder : MonoBehaviour
{
    private void Start()
    {
        if (ZoneManager.Instance != null)
            ZoneManager.Instance.OnCompanionZoneChanged += HandleZoneChange;

        if (EmotionSwitcher.Instance != null)
            EmotionSwitcher.Instance.OnEmotionChanged += HandleEmotionChanged;
    }

    private void OnDisable()
    {
        if (ZoneManager.Instance != null)
            ZoneManager.Instance.OnCompanionZoneChanged -= HandleZoneChange;

        if (EmotionSwitcher.Instance != null)
            EmotionSwitcher.Instance.OnEmotionChanged -= HandleEmotionChanged;
    }

    private void HandleZoneChange(ZoneTag newZone)
    {
        Debug.Log($"CompanionQuipResponder: Companion zone changed to {newZone}");
        QuipManager.Instance.SetZone(newZone); // so filtering works
        QuipManager.Instance.TryPlayQuip(QuipTriggerType.OnZoneEnter);
    }

    private void HandleEmotionChanged(EmotionTag newEmotion)
    {
        Debug.Log($"CompanionQuipResponder: Emotion changed to {newEmotion}, firing quip.");
        QuipManager.Instance.SetEmotion(newEmotion);
        QuipManager.Instance.TryPlayQuip(QuipTriggerType.OnEmotionSwitch);
    }
}