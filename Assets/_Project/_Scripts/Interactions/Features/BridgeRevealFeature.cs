using UnityEngine;
using System.Collections;

public class BridgeRevealFeature : PuzzleFeatureBase
{
    [SerializeField] private GameObject bridgeVisual;
    [SerializeField] private Collider2D bridgeCollider;
    [SerializeField] private float fadeDuration = 0.5f;

    public override void OnInteract(IPuzzleInteractor actor)
    {
        if (isSolved) return;

        isSolved = true;
        StartCoroutine(FadeInBridge());
        bridgeCollider.enabled = true;

        NotifyPuzzleInteractionSuccess();
        RunFeatureEffects(actor);
    }

    private IEnumerator FadeInBridge()
    {
        if (bridgeVisual.TryGetComponent(out SpriteRenderer sr))
        {
            float t = 0f;
            Color c = sr.color;
            c.a = 0;
            sr.color = c;
            sr.enabled = true;

            while (t < 1f)
            {
                t += Time.deltaTime / fadeDuration;
                c.a = Mathf.Lerp(0f, 1f, t);
                sr.color = c;
                yield return null;
            }
        }
    }
}

