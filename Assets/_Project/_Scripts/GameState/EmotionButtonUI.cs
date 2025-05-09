using UnityEngine;
using UnityEngine.UI;

public class EmotionButtonUI : MonoBehaviour
{
    [SerializeField] private EmotionTag emotionToSwitchTo;
    [SerializeField] private Button button;

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
    }

    public void TriggerEmotion()
    {
        if (EmotionSwitcher.Instance != null)
        {
            EmotionSwitcher.Instance.SetEmotion(emotionToSwitchTo);
        }
    }
}
