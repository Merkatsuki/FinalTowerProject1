using TMPro;
using UnityEngine;

public class PhraseDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI phraseText;

    private string currentPhrase;

    public void DisplayText(string phrase)
    {
        currentPhrase = phrase;
        if (phraseText != null)
        {
            phraseText.text = phrase;
        }
    }

    public string GetCurrentPhrase() => currentPhrase;
}
