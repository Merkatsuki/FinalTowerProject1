using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;
using System.Linq;

public class PuzzleDebugPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text displayText;

    private void Start()
    {
        UpdateDisplay();
    }

    private void Update()
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (displayText == null || PuzzleManager.Instance == null)
            return;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<b>Puzzle Flags:</b>\n");

        // --- Boolean Flags ---
        var boolFlags = PuzzleManager.Instance.GetAllBoolFlags();
        if (boolFlags.Count > 0)
        {
            sb.AppendLine("<u>Boolean Flags:</u>");
            foreach (var flag in boolFlags.OrderBy(x => x))
            {
                sb.AppendLine($"- {flag}");
            }
        }
        else
        {
            sb.AppendLine("No boolean flags set.");
        }

        sb.AppendLine();

        // --- Integer Flags ---
        var intFlags = PuzzleManager.Instance.GetAllIntFlags();
        if (intFlags.Count > 0)
        {
            sb.AppendLine("<u>Integer Flags:</u>");
            foreach (var kvp in intFlags.OrderBy(x => x.Key))
            {
                sb.AppendLine($"- {kvp.Key}: {kvp.Value}");
            }
        }
        else
        {
            sb.AppendLine("No integer flags set.");
        }

        displayText.text = sb.ToString();
    }
}
