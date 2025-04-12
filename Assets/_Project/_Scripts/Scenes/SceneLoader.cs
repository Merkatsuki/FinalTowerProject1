using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [Header("UI Fade")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Optional Delay")]
    [SerializeField] private float delayBeforeLoad = 0.25f;

    private bool isLoading = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    public void LoadScene(string sceneName)
    {
        if (!isLoading)
        {
            GameStateManager.Instance?.SetState(GameState.Loading);
            StartCoroutine(LoadSceneRoutine(sceneName));
        }
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        isLoading = true;

        // Fade to black
        yield return StartCoroutine(Fade(1f));

        yield return new WaitForSeconds(delayBeforeLoad);

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        while (!async.isDone)
        {
            yield return null;
        }

        // Fade from black
        yield return StartCoroutine(Fade(0f));

        isLoading = false;
        GameStateManager.Instance?.SetState(GameState.Gameplay);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeImage == null) yield break;

        fadeImage.raycastTarget = true;
        Color startColor = fadeImage.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeImage.color = Color.Lerp(startColor, targetColor, t / fadeDuration);
            yield return null;
        }

        fadeImage.color = targetColor;
        fadeImage.raycastTarget = targetAlpha > 0f;
    }
}
