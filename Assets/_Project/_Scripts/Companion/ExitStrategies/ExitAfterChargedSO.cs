using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(menuName = "Exit Strategy/After Energy Charge")]
public class ExitAfterChargedSO : ExitStrategySO
{
    private bool charged;
    private float chargeProgress;
    private const float chargeDuration = 2f;
    private float zoneStartIntensity = 1f;
    private float robotTargetIntensity = 1f;
    private float robotStartIntensity = 0f;

    private Light2D zoneLight;
    private Light2D robotLight;
    private Color chargeColor = Color.white;

    public override void OnEnter(CompanionController companion, CompanionClueInteractable target)
    {
        charged = false;
        chargeProgress = 0f;

        if (target.TryGetComponent<EnergyDockingZone>(out var zone))
        {
            zoneLight = zone.GetEnergyLight();
            chargeColor = zone.GetChargeColor();
            zoneStartIntensity = zoneLight != null ? zoneLight.intensity : 1f;
        }

        robotLight = companion.GetRobotLight();
        robotTargetIntensity = companion.GetChargedGlowIntensity();
        robotStartIntensity = robotLight != null ? robotLight.intensity : 0f;

        if (robotLight != null)
        {
            robotLight.color = chargeColor;
        }
    }

    public override bool ShouldExit(CompanionController companion, CompanionClueInteractable target)
    {
        if (charged) return true;

        chargeProgress += Time.deltaTime;
        float t = Mathf.Clamp01(chargeProgress / chargeDuration);

        // Update visuals
        if (zoneLight != null)
            zoneLight.intensity = Mathf.Lerp(zoneStartIntensity, 0f, t);

        if (robotLight != null)
            robotLight.intensity = Mathf.Lerp(robotStartIntensity, robotTargetIntensity, t);

        if (chargeProgress >= chargeDuration)
        {
            charged = true;
            return true;
        }

        return false;
    }

}
