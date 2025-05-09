using System.Collections;
using TMPro;
using UnityEngine;

public class CompanionQuipUI : MonoBehaviour
{
    [SerializeField] private GameObject quipPanel;
    [SerializeField] private TMP_Text quipText;
    [SerializeField] private float displayDuration = 3f;

    public void ShowQuip(string text)
    {
        if (!IsVisibleToCamera()) return;

        quipPanel.SetActive(true);
        quipText.text = text;
        StopAllCoroutines();
        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        quipPanel.SetActive(false);
    }

    private bool IsVisibleToCamera()
    {
        Renderer renderer = GetComponentInChildren<Renderer>();
        return renderer != null && renderer.isVisible;
    }
}