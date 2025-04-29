using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DissolveController : MonoBehaviour
{
    [SerializeField] private float dissolveDuration = 1.0f;
    [SerializeField] private AnimationCurve dissolveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private string dissolveProperty = "_DissolveThreshold";

    private Material material;
    private Coroutine dissolveCoroutine;

    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
    }

    public void TriggerDissolve(System.Action onComplete = null)
    {
        if (dissolveCoroutine != null)
        {
            StopCoroutine(dissolveCoroutine);
        }
        dissolveCoroutine = StartCoroutine(DissolveRoutine(onComplete));
    }

    private IEnumerator DissolveRoutine(System.Action onComplete)
    {
        float elapsed = 0f;
        while (elapsed < dissolveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / dissolveDuration);
            float threshold = dissolveCurve.Evaluate(t);
            material.SetFloat(dissolveProperty, threshold);
            yield return null;
        }
        material.SetFloat(dissolveProperty, 1f);
        onComplete?.Invoke();
    }
}

