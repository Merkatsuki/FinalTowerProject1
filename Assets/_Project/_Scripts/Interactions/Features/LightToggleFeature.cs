using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using System.Collections.Generic;

public class LightToggleFeature : MonoBehaviour, IInteractableFeature
{
    [SerializeField] private Light2D targetLight;
    [SerializeField] private float toggleDuration = 0.5f;
    [SerializeField] private float maxIntensity = 8.5f;

    [Header("Feature Effects")]
    [SerializeField] private List<EffectStrategySO> featureEffects = new();

    private bool isOn = false;
    private Tween activeTween;

    private void Awake()
    {
        if (targetLight == null)
        {
            Transform toggleChild = transform.Find("ToggleableLight");
            if (toggleChild != null)
            {
                targetLight = toggleChild.GetComponent<Light2D>();
            }
        }
    }

    public void OnInteract(IPuzzleInteractor actor)
    {
        Debug.Log($"[LightToggleFeature] OnInteract called by: {actor.GetType().Name}");
        ToggleLight();
        RunFeatureEffects(actor);
    }

    private void ToggleLight()
    {
        if (targetLight == null)
        {
            Debug.LogWarning("[LightToggleFeature] No target light assigned!");
            return;
        }

        if (activeTween != null && activeTween.IsActive())
        {
            activeTween.Kill();
        }

        float startIntensity = targetLight.intensity;
        float targetIntensity = isOn ? 0f : maxIntensity;

        activeTween = DOTween.To(
            () => targetLight.intensity,
            x => targetLight.intensity = x,
            targetIntensity,
            toggleDuration
        );

        isOn = !isOn;
    }

    private void RunFeatureEffects(IPuzzleInteractor actor)
    {
        foreach (var effect in featureEffects)
        {
            if (effect != null && TryGetComponent(out IWorldInteractable interactable))
            {
                effect.ApplyEffect(actor, interactable, InteractionResult.Success);
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
