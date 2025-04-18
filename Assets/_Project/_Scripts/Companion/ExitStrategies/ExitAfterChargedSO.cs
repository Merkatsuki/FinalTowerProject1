using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(menuName = "Exit Strategy/After Energy Charge")]
public class ExitAfterChargedSO : ExitStrategySO
{
    private bool charged;
    private float chargeProgress;
    private const float chargeDuration = 2f;

    private Light2D zoneLight;
    private Light2D robotLight;
    private Color chargeColor = Color.white;

    public override void OnEnter(CompanionController companion, InteractableBase target)
    {
        charged = false;
        chargeProgress = 0f;

        if (target.TryGetComponent<EnergyDockingZone>(out var zone))
        {
            zoneLight = zone.GetEnergyLight();
            chargeColor = zone.GetChargeColor();
        }

        robotLight = companion.GetRobotLight();

        if (robotLight != null)
        {
            robotLight.color = chargeColor;
            robotLight.intensity = 0f;
        }
    }

    public override bool ShouldExit(CompanionController companion, InteractableBase target)
    {
        if (charged) return true;

        chargeProgress += Time.deltaTime;
        float t = Mathf.Clamp01(chargeProgress / chargeDuration);

        // Update visuals
        if (zoneLight != null) zoneLight.intensity = 1f - t;
        if (robotLight != null) robotLight.intensity = t;

        if (chargeProgress >= chargeDuration)
        {
            charged = true;
            return true;
        }

        return false;
    }
}
