using TMPro;
using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private GameObject promptRoot;
    [SerializeField] private TextMeshProUGUI promptText;

    public void Show(string message)
    {
        if (promptText != null) promptText.text = message;
        if (promptRoot != null) promptRoot.SetActive(true);
    }

    public void Hide()
    {
        if (promptRoot != null) promptRoot.SetActive(false);
    }
}
