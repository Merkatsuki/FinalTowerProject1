using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class EnergyPuzzleGate : MonoBehaviour
{
    [SerializeField] private EnergyType requiredEnergy;
    [SerializeField] private UnityEvent onGateActivated;
    [SerializeField] private Light2D gateLight;
    [SerializeField] private float gateGlowIntensity = 20f;

    private bool isActivated = false;

    public void AcceptEnergyFrom(CompanionController companion)
    {
        EnergyType energy = companion.GetEnergyType();

        if (energy != requiredEnergy || isActivated) return;

        if (gateLight != null)
        {
            gateLight.color = EnergyColorMap.GetColor(energy);
            gateLight.intensity = gateGlowIntensity;
        }

        // Keep companion's energy — allow multiple activations
        isActivated = true;
        onGateActivated?.Invoke();
        Debug.Log($"Gate activated with energy: {energy}");
    }

    public EnergyType GetRequiredEnergy() => requiredEnergy;
    public bool IsActivated() => isActivated;
}

