using UnityEngine;
using System;
using System.Collections;

public class EmotionSwitcher : MonoBehaviour
{
    public static EmotionSwitcher Instance { get; private set; }

    public event Action<EmotionTag> OnEmotionChanged;
    public event Action OnEmotionCooldownStarted;
    public event Action OnEmotionCooldownEnded;

    [SerializeField] private float emotionSwitchCooldown = 2.5f;
    private float lastSwitchTime = -999f;

    private EmotionTag currentEmotion = EmotionTag.Neutral;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void SetEmotion(EmotionTag newEmotion)
    {
        if (newEmotion == currentEmotion) return;

        if (Time.time < lastSwitchTime + emotionSwitchCooldown)
        {
            Debug.Log("EmotionSwitcher: Emotion change blocked by cooldown.");
            return;
        }

        lastSwitchTime = Time.time;
        currentEmotion = newEmotion;
        Debug.Log($"EmotionSwitcher: Emotion changed to {newEmotion}");
        OnEmotionChanged?.Invoke(newEmotion);

        OnEmotionCooldownStarted?.Invoke();
        StartCoroutine(CooldownTimer());
    }

    private IEnumerator CooldownTimer()
    {
        yield return new WaitForSeconds(emotionSwitchCooldown);
        OnEmotionCooldownEnded?.Invoke();
    }

    public EmotionTag GetCurrentEmotion() => currentEmotion;
    public float EmotionCooldown => emotionSwitchCooldown;
}
