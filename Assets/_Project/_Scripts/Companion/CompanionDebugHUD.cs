using TMPro;
using UnityEngine;

public class CompanionDebugHUD : MonoBehaviour
{
    [SerializeField] private CompanionController companion;
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private bool showDebug = true;

    private void Update()
    {
        if (!showDebug || companion == null || debugText == null)
        {
            if (debugText != null)
                debugText.text = "";
            return;
        }

        var fsm = companion.GetFSM();
        var perception = companion.GetPerception();

        string state = fsm?.CurrentStateType.ToString() ?? "N/A";
        EnergyType energyType = companion.GetEnergyType();
        bool isLocked = companion.IsInteractionLocked;
        string targetName = perception?.GetCurrentTarget()?.GetTransform().name ?? "None";

        string energyColored = GetEnergyTextColored(energyType);

        string info =
            $"<b>Companion Debug</b>\n" +
            $"State: <color=yellow>{state}</color>\n" +
            $"Energy: {energyColored}\n" +
            $"Interaction Locked: <color={(isLocked ? "red" : "green")}>{isLocked}</color>\n" +
            $"Target: <color=white>{targetName}</color>";

        debugText.text = info;
    }

    private string GetEnergyTextColored(EnergyType type)
    {
        Color color = EnergyColorMap.GetColor(type);
        string hex = ColorUtility.ToHtmlStringRGB(color);
        return $"<color=#{hex}>{type}</color>";
    }
}
