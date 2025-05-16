using Momentum;
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
        var emotion = EmotionSwitcher.Instance?.GetCurrentEmotion().ToString() ?? "Unknown";

        string state = fsm?.CurrentStateType.ToString() ?? "N/A";
        bool isLocked = companion.IsInteractionLocked;
        bool isCommandMode = InputManager.instance?.IsCommandMode ?? false;
        string targetName = companion.GetCurrentTrackedTarget()?.GetDisplayName() ?? "None";
        string commandedTarget = companion.HasPendingPlayerCommand() ? "Yes" : "No";

        string info =
            $"<b>Companion Debug</b>\n" +
            $"State: <color=yellow>{state}</color>\n" +
            $"Emotion: <color=cyan>{emotion}</color>\n" +
            $"Interaction Locked: <color={(isLocked ? "red" : "green")}>{isLocked}</color>\n" +
            $"Command Mode: <color={(isCommandMode ? "green" : "gray")}>{isCommandMode}</color>\n" +
            $"Target: <color=white>{targetName}</color>\n" +
            $"Player Command Issued: <color=white>{commandedTarget}</color>";

        debugText.text = info;
    }
}
