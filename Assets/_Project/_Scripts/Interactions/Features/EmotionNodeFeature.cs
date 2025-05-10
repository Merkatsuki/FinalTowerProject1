// Refactored EmotionNodeFeature.cs to inherit from PuzzleFeatureBase
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using System.Collections;

public class EmotionNodeFeature : PuzzleFeatureBase
{
    [Header("Emotion Settings")]
    [SerializeField] private EmotionTag requiredEmotion = EmotionTag.Neutral;
    [SerializeField] private bool startCharged = false;
    [SerializeField] private bool autoDecay = true;

    [Header("Light Feedback")]
    [SerializeField] private Light2D emotionLight;
    [SerializeField] private float maxIntensity = 1.5f;
    [SerializeField] private float chargeDuration = 1.5f;
    [SerializeField] private float decayDelay = 5f;
    [SerializeField] private float decayDuration = 2f;

    private bool isCharged = false;
    private Tween activeTween;
    private Coroutine decayCoroutine;

    private void Awake()
    {
        if (emotionLight == null)
        {
            Transform child = transform.Find("EmotionLight");
            if (child != null)
                emotionLight = child.GetComponent<Light2D>();
        }
        UpdateLightColor();
    }

    private void Start()
    {
        isCharged = startCharged;
        if (emotionLight != null)
        {
            emotionLight.intensity = isCharged ? maxIntensity : 0f;
            if (isCharged && autoDecay)
                decayCoroutine = StartCoroutine(DecayAfterDelay());
        }
    }

    public override void OnInteract(IPuzzleInteractor actor)
    {
        if (isCharged) return;

        EmotionTag current = EmotionSwitcher.Instance.GetCurrentEmotion();
        if (current == requiredEmotion || requiredEmotion == EmotionTag.Any)
        {
            Charge(actor);
        }
        else
        {
            Debug.Log($"[EmotionNode] Incorrect emotion. Required: {requiredEmotion}, current: {current}");
        }
    }

    public void Charge(IPuzzleInteractor actor)
    {
        isCharged = true;

        if (emotionLight != null)
        {
            if (activeTween != null && activeTween.IsActive())
                activeTween.Kill();

            activeTween = DOTween.To(
                () => emotionLight.intensity,
                x => emotionLight.intensity = x,
                maxIntensity,
                chargeDuration
            );
        }

        RunFeatureEffects(actor);
        isSolved = true;
        NotifyPuzzleInteractionSuccess();

        if (autoDecay)
        {
            if (decayCoroutine != null)
                StopCoroutine(decayCoroutine);

            decayCoroutine = StartCoroutine(DecayAfterDelay());
        }

        Debug.Log("[EmotionNode] Charged successfully.");
    }

    private IEnumerator DecayAfterDelay()
    {
        yield return new WaitForSeconds(decayDelay);
        Discharge();
    }

    public void Discharge()
    {
        isCharged = false;

        if (emotionLight != null)
        {
            if (activeTween != null && activeTween.IsActive())
                activeTween.Kill();

            activeTween = DOTween.To(
                () => emotionLight.intensity,
                x => emotionLight.intensity = x,
                0f,
                decayDuration
            );
        }

        Debug.Log("[EmotionNode] Discharged.");
    }

    private void UpdateLightColor()
    {
        if (emotionLight != null)
        {
            emotionLight.color = EmotionColorMap.GetColor(requiredEmotion);
        }
    }

    public void SetEmotionLight(Light2D light)
    {
        emotionLight = light;
        UpdateLightColor();
    }

    public override void ResetPuzzleComponent()
    {
        base.ResetPuzzleComponent();
        Discharge();
    }

    public bool IsCharged() => isCharged;
}
