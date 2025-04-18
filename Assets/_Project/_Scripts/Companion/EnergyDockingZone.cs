using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Collider2D))]
public class EnergyDockingZone : MonoBehaviour
{
    [SerializeField] private Light2D energyLight;
    [SerializeField] private float chargeTime = 3f;
    [SerializeField] private float lightIntensity = 20f;

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
        isCharging = true;

        Light2D companionLight = companion.GetRobotLight();
        Color energyColor = EnergyColorMap.GetColor(zoneType); // a helper script you define

        float elapsed = 0f;
        while (elapsed < chargeTime)
        {
            float t = elapsed / chargeTime;
            if (energyLight != null)
                energyLight.intensity = Mathf.Lerp(lightIntensity, 0f, t);

            if (companionLight != null)
            {
                companionLight.intensity = Mathf.Lerp(0f, 50f, t);
                companionLight.color = energyColor;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (energyLight != null) energyLight.intensity = 0f;
        if (companionLight != null)
        {
            companionLight.intensity = 50f;
            companionLight.color = energyColor;
        }

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