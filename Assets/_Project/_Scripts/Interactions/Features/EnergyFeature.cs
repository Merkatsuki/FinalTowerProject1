using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnergyFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Energy Settings")]
    [SerializeField] private bool isCharged = false;
    [SerializeField] private EnergyType energyType = EnergyType.None;

    private Light2D energyLight;
    private float maxLightIntensity = 1.5f;
    private float dischargeSpeed = 0.5f;

    private void Awake()
    {
        energyLight = GetComponentInChildren<Light2D>();
        if (energyLight == null)
        {
            GameObject lightObj = new GameObject("EnergyLight");
            lightObj.transform.SetParent(transform);
            lightObj.transform.localPosition = Vector3.zero;
            energyLight = lightObj.AddComponent<Light2D>();
            energyLight.lightType = Light2D.LightType.Point;
            energyLight.pointLightOuterRadius = 2.5f;
            energyLight.intensity = 0f;
            Debug.Log("[EnergyFeature] Auto-created Energy Light.");
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
        // Potential expansion: actor can charge or discharge
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

    public bool IsCharged() => isCharged;
}