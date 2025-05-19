using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Rendering.Universal;

public class LightToggleFeature : FeatureBase
{
    [Header("Light Settings")]
    [SerializeField] private List<Light2D> targetLights = new();

    [Header("Simple Toggle Mode")]
    [SerializeField] private float onIntensity = 1f;
    [SerializeField] private float offIntensity = 0f;
    [SerializeField] private float transitionDuration = 0.5f;

    [Header("Dimmer Switch Mode")]
    [SerializeField] private bool useMultipleIntensityLevels = false;
    [SerializeField] private List<float> intensityLevels = new();
    [SerializeField] private float dimmerTransitionDuration = 0.5f;

    [SerializeField] private List<FeatureBase> connectedFeatures;

    private bool isOn = false;
    private int currentIntensityIndex = 0;
    private List<Tween> activeTweens = new();

    private void Start()
    {
        if (targetLights == null || targetLights.Count == 0)
        {
            Debug.LogWarning("[LightToggleFeature] No Light2D targets assigned!");
            return;
        }

        if (useMultipleIntensityLevels && intensityLevels.Count > 0)
        {
            currentIntensityIndex = 0;
            foreach (var light in targetLights)
            {
                if (light != null)
                    light.intensity = intensityLevels[0];
            }
        }
        else
        {
            foreach (var light in targetLights)
            {
                if (light != null)
                    light.intensity = offIntensity;
            }
        }
    }

    public override void OnInteract(IPuzzleInteractor actor)
    {
        ToggleLight();
    }

    public void SetPowered(bool powered)
    {
        Debug.Log($"[LightToggleFeature] Set powered state: {powered} this feature is set to {isOn} ");
        if (powered == isOn) return; // Already in desired state

        Debug.Log($"[LightToggleFeature] Toggling light to: {powered}");
        ToggleLight(); // Flip to match desired state
    }

    private void ToggleLight()
    {
        if (targetLights == null || targetLights.Count == 0)
        {
            Debug.LogWarning("[LightToggleFeature] No Light2D targets to toggle.");
            return;
        }

        KillActiveTweens();

        if (useMultipleIntensityLevels && intensityLevels.Count > 0)
        {
            currentIntensityIndex = (currentIntensityIndex + 1) % intensityLevels.Count;
            float nextIntensity = intensityLevels[currentIntensityIndex];

            foreach (var light in targetLights)
            {
                if (light == null) continue;

                activeTweens.Add(DOTween.To(
                    () => light.intensity,
                    x => light.intensity = x,
                    nextIntensity,
                    dimmerTransitionDuration
                ));
            }
        }
        else
        {
            float targetIntensity = isOn ? offIntensity : onIntensity;

            foreach (var light in targetLights)
            {
                if (light == null) continue;

                activeTweens.Add(DOTween.To(
                    () => light.intensity,
                    x => light.intensity = x,
                    targetIntensity,
                    transitionDuration
                ));
            }

            isOn = !isOn;
        }

        NotifyConnectedFeatures();
        RunFeatureEffects();
    }

    private void KillActiveTweens()
    {
        foreach (var tween in activeTweens)
        {
            if (tween != null && tween.IsActive()) tween.Kill();
        }
        activeTweens.Clear();
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

    private void NotifyConnectedFeatures()
    {
        foreach (var feature in connectedFeatures)
            feature?.OnInteract(ReferenceManager.Instance.Player); // Null = system-triggered
    }

    public void SetTargetLights(List<Light2D> lights)
    {
        targetLights = lights;
    }

    public void AddTargetLight(Light2D light)
    {
        if (!targetLights.Contains(light))
            targetLights.Add(light);
    }
}

