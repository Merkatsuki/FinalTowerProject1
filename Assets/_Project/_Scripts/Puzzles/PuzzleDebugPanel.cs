using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class PuzzleDebugPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private Button refreshButton;

    private void Start()
    {
        if (refreshButton != null)
            refreshButton.onClick.AddListener(UpdateDisplay);

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (displayText == null) return;
    }
}