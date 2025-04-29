using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    private static ScreenFader instance;
    public static ScreenFader Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void FadeOut(Action onComplete = null)
    {
        StartCoroutine(FadeRoutine(0f, 1f, onComplete));
    }

    public void FadeIn(Action onComplete = null)
    {
        StartCoroutine(FadeRoutine(1f, 0f, onComplete));
    }

    private IEnumerator FadeRoutine(float startAlpha, float endAlpha, Action onComplete)
    {
        float elapsed = 0f;
        Color c = fadeImage.color;
        c.a = startAlpha;
        fadeImage.color = c;
        fadeImage.gameObject.SetActive(true);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            c.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeImage.color = c;
            yield return null;
        }

        c.a = endAlpha;
        fadeImage.color = c;

        if (endAlpha == 0f)
        {
            fadeImage.gameObject.SetActive(false);
        }

        onComplete?.Invoke();
    }
}


