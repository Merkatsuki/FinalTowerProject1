using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Rendering.Universal;

public class LightToggleFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Light Settings")]
    [SerializeField] private Light2D targetLight;

    [Header("Simple Toggle Mode")]
    [SerializeField] private bool startOn = true;
    [SerializeField] private float onIntensity = 1f;
    [SerializeField] private float offIntensity = 0f;
    [SerializeField] private float transitionDuration = 0.5f;

    [Header("Dimmer Switch Mode")]
    [SerializeField] private bool useMultipleIntensityLevels = false;
    [SerializeField] private List<float> intensityLevels = new();
    [SerializeField] private float dimmerTransitionDuration = 0.5f;

    [Header("Feature Effects")]
    [SerializeField] private List<EffectStrategySO> featureEffects = new();

    private bool isOn = true;
    private int currentIntensityIndex = 0;
    private Tween activeTween;

    private void Start()
    {
        if (targetLight == null)
        {
            Debug.LogWarning("[LightToggleFeature] No Light2D assigned!");
            return;
        }

        isOn = startOn;

        if (useMultipleIntensityLevels && intensityLevels.Count > 0)
        {
            // Start at first intensity level
            targetLight.intensity = intensityLevels[0];
            currentIntensityIndex = 0;
        }
        else
        {
            targetLight.intensity = startOn ? onIntensity : offIntensity;
        }
    }

    public void OnInteract(IPuzzleInteractor actor)
    {
        ToggleLight();
    }

    private void ToggleLight()
    {
        if (targetLight == null)
        {
            Debug.LogWarning("[LightToggleFeature] No Light2D to toggle.");
            return;
        }

        if (activeTween != null && activeTween.IsActive())
        {
            activeTween.Kill();
        }

        if (useMultipleIntensityLevels && intensityLevels.Count > 0)
        {
            currentIntensityIndex = (currentIntensityIndex + 1) % intensityLevels.Count;
            float nextIntensity = intensityLevels[currentIntensityIndex];

            activeTween = DOTween.To(
                () => targetLight.intensity,
                x => targetLight.intensity = x,
                nextIntensity,
                dimmerTransitionDuration
            );
        }
        else
        {
            float startIntensity = targetLight.intensity;
            float targetIntensity = isOn ? offIntensity : onIntensity;

            activeTween = DOTween.To(
                () => targetLight.intensity,
                x => targetLight.intensity = x,
                targetIntensity,
                transitionDuration
            );

            isOn = !isOn;
        }

        RunFeatureEffects();
    }

    private void RunFeatureEffects()
    {
        foreach (var effect in featureEffects)
        {
            if (effect != null && TryGetComponent(out IWorldInteractable interactable))
            {
                effect.ApplyEffect(null, interactable, InteractionResult.Success);
            }
        }
    }

    public void SetFeatureEffects(List<EffectStrategySO> effects)
    {
        featureEffects = effects ?? new List<EffectStrategySO>();
    }

    public void SetToggleLight(Light2D light)
    {
        targetLight = light;
    }
}
