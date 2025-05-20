using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FloatingWordReactiveEffect : MonoBehaviour
{
    [Header("Floating Motion")]
    [SerializeField] private float driftSpeed = 0.5f;
    [SerializeField] private float floatAmplitudeY = 5f;
    [SerializeField] private float floatAmplitudeX = 3f;
    [SerializeField] private float xDriftSpeed = 0.3f;

    [Header("Perlin Noise Settings")]
    [SerializeField] private float noiseScale = 1.0f;
    [SerializeField] private float noiseStrength = 1.0f;
    [SerializeField] private float noiseOffsetSeed = 0f;

    [Header("Fade Settings")]
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private float minAlpha = 0.5f;
    [SerializeField] private float maxAlpha = 1f;

    [Header("Scale Pulse")]
    [SerializeField] private float scaleAmount = 0.05f;
    [SerializeField] private float scaleSpeed = 0.9f;

    [Header("Correct Pair Feedback")]
    [SerializeField] private bool correctEffectActive = false;
    [SerializeField] private float glowPulseAmount = 0.1f;

    [Header("Player Influence Settings")]
    [SerializeField] private bool reactToPlayer = false;
    [SerializeField] private Transform player;
    [SerializeField] private float influenceRadius = 3f;
    [SerializeField] private float attractionForce = 0f; // positive = attract, negative = repel
    [SerializeField] private float effectStrengthMultiplier = 1f; // Multiplies fade, drift, scale


    private TextMeshProUGUI text;
    private Vector3 startPos;
    private float timer;
    private bool isCurrentlyCorrect = false;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        startPos = transform.localPosition;
        timer = Random.Range(0f, 100f);

        if (reactToPlayer && player == null)
        {
            player = ReferenceManager.Instance.Player.transform;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        float xOffset = Mathf.Sin(timer * xDriftSpeed) * floatAmplitudeX +
                        (Mathf.PerlinNoise(timer * noiseScale, noiseOffsetSeed) * 2f - 1f) * noiseStrength;

        float yOffset = Mathf.Sin(timer * driftSpeed) * floatAmplitudeY;
        Vector3 finalOffset = new Vector3(xOffset, yOffset, 0f);
        float proximityBoost = 1f;

        if (reactToPlayer && player != null)
        {
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance < influenceRadius)
            {
                // Directional motion offset
                Vector3 direction = (transform.position - player.position).normalized;
                float force = Mathf.Lerp(attractionForce, 0, distance / influenceRadius);
                finalOffset += direction * force;

                // Optional strength modifier
                float t = 1f - (distance / influenceRadius);
                proximityBoost = Mathf.Lerp(1f, effectStrengthMultiplier, t);
            }
        }

        transform.localPosition = startPos + finalOffset;

        // Scale pulse
        float scale = 1 + Mathf.Sin(timer * scaleSpeed) * scaleAmount * proximityBoost;
        transform.localScale = new Vector3(scale, scale, 1f);

        // Fade modulation
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(timer * fadeSpeed) + 1f) / 2f) * proximityBoost;
        Color c = text.color;
        text.color = new Color(c.r, c.g, c.b, Mathf.Clamp01(alpha));

        if (isCurrentlyCorrect)
        {
            float glowScale = 1 + Mathf.Sin(timer * 2f) * glowPulseAmount;
            transform.localScale *= glowScale;
        }
    }

    public void SetActiveEffect(bool isCorrect)
    {
        isCurrentlyCorrect = isCorrect;
    }
}
