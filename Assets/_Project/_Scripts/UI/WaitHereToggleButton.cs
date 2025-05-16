using UnityEngine;
using UnityEngine.UI;

public class WaitHereToggleButton : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private Image iconImage;
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color inactiveColor = Color.white;

    private void Awake()
    {
        if (toggle == null)
            toggle = GetComponent<Toggle>();

        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void Start()
    {
        // Sync initial toggle state with command manager if needed
        toggle.isOn = CompanionCommandManager.Instance?.IsWaitHereToggled() ?? false;
        UpdateVisual(toggle.isOn);
    }

    private void OnToggleChanged(bool isOn)
    {
        CompanionCommandManager.Instance?.ToggleWaitHereMode(isOn);
        QuipManager.Instance?.TryPlayWaitHereQuip(null);
        UpdateVisual(isOn);
    }

    private void UpdateVisual(bool isActive)
    {
        if (iconImage != null)
            iconImage.color = isActive ? activeColor : inactiveColor;
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnToggleChanged);
    }
}