using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BridgeRevealFeature : FeatureBase
{
    [Header("Bridge Components")]
    [SerializeField] private List<GameObject> bridgeVisuals; // Now supports multiple visuals
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
        currentFade = StartCoroutine(FadeBridges(isVisible));

        bridgeCollider.enabled = isVisible;

        NotifyPuzzleInteractionSuccess();
        RunFeatureEffects(actor);
    }

    private IEnumerator FadeBridges(bool fadeIn)
    {
        float t = 0f;
        List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

        foreach (var go in bridgeVisuals)
        {
            if (go.TryGetComponent(out SpriteRenderer sr))
            {
                sr.enabled = true;
                spriteRenderers.Add(sr);
            }
        }

        if (spriteRenderers.Count == 0) yield break;

        float startAlpha = spriteRenderers[0].color.a; // Assume all are in sync
        float endAlpha = fadeIn ? 1f : 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            foreach (var sr in spriteRenderers)
            {
                Color c = sr.color;
                c.a = alpha;
                sr.color = c;
            }

            yield return null;
        }

        if (!fadeIn)
        {
            foreach (var sr in spriteRenderers)
            {
                sr.enabled = false;
            }
        }
    }
}
