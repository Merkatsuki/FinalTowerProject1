using UnityEngine;
using System.Collections;

public class BridgeRevealFeature : FeatureBase
{
    [Header("Bridge Components")]
    [SerializeField] private GameObject bridgeVisual;
    [SerializeField] private Collider2D bridgeCollider;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private bool isToggleable = false;

    private bool isVisible = false;
    private Coroutine currentFade;

    public override void OnInteract(IPuzzleInteractor actor)
    {
        if (!isToggleable && isSolved) return;

        // Toggle or set permanently
        isVisible = isToggleable ? !isVisible : true;
        isSolved = isVisible;

        if (currentFade != null)
            StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeBridge(isVisible));

        bridgeCollider.enabled = isVisible;

        NotifyPuzzleInteractionSuccess();
        RunFeatureEffects(actor);
    }

    private IEnumerator FadeBridge(bool fadeIn)
    {
        if (bridgeVisual.TryGetComponent(out SpriteRenderer sr))
        {
            float t = 0f;
            float startAlpha = sr.color.a;
            float endAlpha = fadeIn ? 1f : 0f;
            Color c = sr.color;

            sr.enabled = true;

            while (t < 1f)
            {
                t += Time.deltaTime / fadeDuration;
                c.a = Mathf.Lerp(startAlpha, endAlpha, t);
                sr.color = c;
                yield return null;
            }

            if (!fadeIn)
                sr.enabled = false;
        }
    }
}
