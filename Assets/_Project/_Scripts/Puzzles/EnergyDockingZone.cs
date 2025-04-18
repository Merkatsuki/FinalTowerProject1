using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Collider2D))]
public class EnergyDockingZone : MonoBehaviour
{
    [SerializeField] private Light2D energyLight;
    [SerializeField] private float chargeTime = 3f;

    [Tooltip("Optional point where the robot should position itself.")]
    public Transform dockingPoint;
    public EnergyType zoneType;


    public void Dock(CompanionController companion)
    {
        if (isCharging) return;
        StartCoroutine(ChargeSequence(companion));
    }

    private bool isCharging = false;

    private IEnumerator ChargeSequence(CompanionController companion)
    {
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
        isCharging = false;
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