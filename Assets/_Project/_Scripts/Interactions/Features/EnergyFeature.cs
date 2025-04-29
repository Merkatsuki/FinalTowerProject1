using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class EnergyFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Energy Settings")]
    [SerializeField] private bool isCharged = false;
    [SerializeField] private EnergyType energyType = EnergyType.None;

    [Header("Feature Effects")]
    [SerializeField] private List<EffectStrategySO> featureEffects = new();

    private Light2D energyLight;
    private float maxLightIntensity = 1.5f;
    private float dischargeSpeed = 0.5f;

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

    private void Update()
    {
        if (!isCharged && energyLight != null && energyLight.intensity > 0f)
        {
            energyLight.intensity = Mathf.Max(0f, energyLight.intensity - dischargeSpeed * Time.deltaTime);
        }
    }

    public void OnInteract(IPuzzleInteractor actor)
    {
        Debug.Log($"[EnergyFeature] OnInteract called by: {actor.GetType().Name}");
        // Future expansion (charge by actor?)
        RunFeatureEffects(actor);
    }

    public void Charge(EnergyType incomingEnergyType)
    {
        if (energyType != EnergyType.None && incomingEnergyType != energyType)
        {
            Debug.Log("[EnergyFeature] Incorrect energy type.");
            return;
        }

        isCharged = true;
        if (energyLight != null)
        {
            energyLight.intensity = maxLightIntensity;
        }

        Debug.Log("[EnergyFeature] Energy charged!");
    }

    public void Discharge()
    {
        isCharged = false;
        Debug.Log("[EnergyFeature] Energy discharged.");
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
