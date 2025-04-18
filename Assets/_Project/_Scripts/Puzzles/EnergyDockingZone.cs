using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Collider2D))]
public class EnergyDockingZone : MonoBehaviour
{
    [SerializeField] private Light2D energyLight;
    [SerializeField] private float chargeTime = 3f;
    [SerializeField] private float rechargeDuration = 5f;
    [SerializeField] private float maxIntensity = 20f;

    [Tooltip("Optional point where the robot should position itself.")]
    public Transform dockingPoint;
    public EnergyType zoneType;

    private bool isCharging = false;

    private void Start()
    {
        if (energyLight != null)
        {
            energyLight.intensity = maxIntensity;
            energyLight.color = GetChargeColor();
        }
    }

    public void Dock(CompanionController companion)
    {
        if (isCharging) return;
        StartCoroutine(ChargeSequence(companion));
    }

    private IEnumerator ChargeSequence(CompanionController companion)
    {
        isCharging = true;
        Color energyColor = EnergyColorMap.GetColor(zoneType);

        if (energyLight != null)
        {
            float initialIntensity = energyLight.intensity;
            float elapsed = 0f;

            while (elapsed < chargeTime)
            {
                float t = elapsed / chargeTime;
                energyLight.intensity = Mathf.Lerp(initialIntensity, 0f, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            energyLight.intensity = 0f;
        }

        StartCoroutine(companion.ChargeGlow(energyColor, chargeTime));
        companion.SetEnergyType(zoneType);
        StartCoroutine(RechargeDockLight());
    }

    private IEnumerator RechargeDockLight()
    {
        float elapsed = 0f;
        while (elapsed < rechargeDuration)
        {
            float t = elapsed / rechargeDuration;
            if (energyLight != null)
                energyLight.intensity = Mathf.Lerp(0f, maxIntensity, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (energyLight != null)
            energyLight.intensity = maxIntensity;

        isCharging = false;

        if (TryGetComponent<CompanionClueInteractable>(out var clue))
        {
            clue.ResetHandled();
        }
    }

    public Light2D GetEnergyLight() => energyLight;

    public Color GetChargeColor()
    {
        return EnergyColorMap.GetColor(zoneType);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(dockingPoint ? dockingPoint.position : transform.position, 0.3f);
    }
}
