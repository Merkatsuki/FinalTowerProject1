using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class EnergyFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Energy Settings")]
    [SerializeField] private bool isCharged = false;
    [SerializeField] private EnergyType energyType = EnergyType.None;
    [SerializeField] private bool startCharged = true;

    [Header("Light Behavior")]
    [SerializeField] private Light2D energyLight;
    [SerializeField] private float maxLightIntensity = 1.5f;
    [SerializeField] private float chargeDuration = 1.5f;
    [SerializeField] private float decayDelay = 5f;
    [SerializeField] private float decayDuration = 2f;
    [SerializeField] private bool autoDecay = true;

    [Header("Feature Effects")]
    [SerializeField] private List<EffectStrategySO> featureEffects = new();

    private Tween activeTween;
    private Coroutine decayCoroutine;

    private void Awake()
    {
        if (energyLight == null)
        {
            Transform energyChild = transform.Find("EnergyLight");
            if (energyChild != null)
            {
                energyLight = energyChild.GetComponent<Light2D>();
            }
        }

        UpdateEnergyLightColor();
    }

    private void Start()
    {
        if (energyLight != null)
        {
            isCharged = startCharged;
            energyLight.intensity = isCharged ? maxLightIntensity : 0f;

            if (isCharged && autoDecay)
            {
                decayCoroutine = StartCoroutine(DecayAfterDelay());
            }
        }
    }

    public void OnInteract(IPuzzleInteractor actor)
    {
        Debug.Log($"[EnergyFeature] OnInteract called by: {actor.GetType().Name}");

        if (!isCharged)
        {
            Charge(actor);
        }
        else
        {
            Discharge(actor);
        }
    }

    public void Charge(IPuzzleInteractor actor)
    {
        isCharged = true;

        if (energyLight != null)
        {
            if (activeTween != null && activeTween.IsActive())
                activeTween.Kill();

            activeTween = DOTween.To(
                () => energyLight.intensity,
                x => energyLight.intensity = x,
                maxLightIntensity,
                chargeDuration
            );
        }

        Debug.Log("[EnergyFeature] Energy charged!");

        RunFeatureEffects(actor);

        if (autoDecay)
        {
            if (decayCoroutine != null)
                StopCoroutine(decayCoroutine);

            decayCoroutine = StartCoroutine(DecayAfterDelay());
        }
    }

    public void Discharge(IPuzzleInteractor actor = null)
    {
        isCharged = false;

        if (energyLight != null)
        {
            if (activeTween != null && activeTween.IsActive())
                activeTween.Kill();

            activeTween = DOTween.To(
                () => energyLight.intensity,
                x => energyLight.intensity = x,
                0f,
                decayDuration
            );
        }

        Debug.Log("[EnergyFeature] Energy discharged!");

        RunFeatureEffects(actor);
    }

    private IEnumerator DecayAfterDelay()
    {
        yield return new WaitForSeconds(decayDelay);
        Discharge();
    }

    private void UpdateEnergyLightColor()
    {
        if (energyLight != null)
        {
            energyLight.color = EnergyColorMap.GetColor(energyType);
        }
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

    public bool IsCharged() => isCharged;

    public void SetEnergyLight(Light2D light)
    {
        energyLight = light;
        UpdateEnergyLightColor();
    }
}
