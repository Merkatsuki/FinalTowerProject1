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
        bool isLocked = companion.IsInteractionLocked;
        string targetName = perception?.GetCurrentTarget()?.GetTransform().name ?? "None";


        string info =
            $"<b>Companion Debug</b>\n" +
            $"State: <color=yellow>{state}</color>\n" +
            $"Interaction Locked: <color={(isLocked ? "red" : "green")}>{isLocked}</color>\n" +
            $"Target: <color=white>{targetName}</color>";

        debugText.text = info;
    }

}
