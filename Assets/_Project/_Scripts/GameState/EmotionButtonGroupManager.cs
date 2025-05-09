using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmotionButtonGroupManager : MonoBehaviour
{
    [SerializeField] private List<Button> emotionButtons = new();

    private void Start()
    {
        if (EmotionSwitcher.Instance != null)
        {
            EmotionSwitcher.Instance.OnEmotionCooldownStarted += DisableAllButtons;
            EmotionSwitcher.Instance.OnEmotionCooldownEnded += EnableAllButtons;
        }
    }

    private void OnDisable()
    {
        if (EmotionSwitcher.Instance != null)
        {
            EmotionSwitcher.Instance.OnEmotionCooldownStarted -= DisableAllButtons;
            EmotionSwitcher.Instance.OnEmotionCooldownEnded -= EnableAllButtons;
        }
    }

    private void DisableAllButtons()
    {
        foreach (var btn in emotionButtons)
        {
            if (btn != null)
                btn.interactable = false;
        }
    }

    private void EnableAllButtons()
    {
        foreach (var btn in emotionButtons)
        {
            if (btn != null)
                btn.interactable = true;
        }
    }
}
