// Updated CompanionQuipResponder.cs to include idle and direction spam logic
using Momentum;
using UnityEngine;

public class CompanionQuipResponder : MonoBehaviour
{
    [Header("Idle & Direction Spam Settings")]
    [SerializeField] private float idleTimeThreshold = 8f;
    [SerializeField] private int directionSpamThreshold = 6;
    [SerializeField] private float directionSpamWindow = 2f;

    private float idleTimer = 0f;
    private float directionTimer = 0f;
    private float lastDirection = 0f;
    private int directionSwitchCount = 0;

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

    private void Update()
    {
        Vector2 input = InputManager.instance.WASDInput;

        // Idle detection
        if (input.magnitude < 0.1f)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > idleTimeThreshold)
            {
                QuipManager.Instance.TryPlayQuip(QuipTriggerType.OnIdle);
                idleTimer = -999f; // lockout after firing
            }
        }
        else
        {
            idleTimer = 0f;
        }

        // Direction spam detection
        directionTimer += Time.deltaTime;
        float currentDir = Mathf.Sign(input.x);

        if (currentDir != 0 && currentDir != lastDirection)
        {
            directionSwitchCount++;
            lastDirection = currentDir;
        }

        if (directionTimer >= directionSpamWindow)
        {
            if (directionSwitchCount >= directionSpamThreshold)
            {
                QuipManager.Instance.TryPlayQuip(QuipTriggerType.OnDirectionSpam);
            }
            directionSwitchCount = 0;
            directionTimer = 0f;
        }
    }

    private void HandleZoneChange(ZoneTag newZone)
    {
        Debug.Log($"CompanionQuipResponder: Companion zone changed to {newZone}");
        QuipManager.Instance.SetZone(newZone);
        QuipManager.Instance.TryPlayQuip(QuipTriggerType.OnZoneEnter);
    }

    private void HandleEmotionChanged(EmotionTag newEmotion)
    {
        Debug.Log($"CompanionQuipResponder: Emotion changed to {newEmotion}, firing quip.");
        QuipManager.Instance.SetEmotion(newEmotion);
        QuipManager.Instance.TryPlayQuip(QuipTriggerType.OnEmotionSwitch);
    }
}
