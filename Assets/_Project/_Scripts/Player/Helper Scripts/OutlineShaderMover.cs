using UnityEngine;
using Momentum;

[RequireComponent(typeof(SpriteRenderer))]
public class OutlineShaderMover : MonoBehaviour
{
    [Header("Noise Scroll Speed")]
    [Tooltip("How fast the noise scrolls horizontally.")]
    public float horizontalNoiseSpeedScale = 0.05f;

    [Tooltip("How fast the noise scrolls vertically.")]
    public float verticalNoiseSpeedScale = 0.05f;

    [Header("Noise Scale Control")]
    [Tooltip("Base noise scale applied at zero speed.")]
    public Vector2 baseNoiseScale = new Vector2(1f, 1f);

    [Tooltip("How much movement speed influences noise scale.")]
    public float scaleSensitivity = 0.5f;

    [Tooltip("If true, scale increases with speed. If false, scale decreases with speed.")]
    public bool scaleIncreasesWithSpeed = true;

    private SpriteRenderer spriteRenderer;
    private Material material;
    private PlayerReferences playerReferences;

    private static readonly int NoiseSpeedID = Shader.PropertyToID("_OuterOutlineNoiseSpeed");
    private static readonly int NoiseScaleID = Shader.PropertyToID("_OuterOutlineNoiseScale");

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = new Material(spriteRenderer.material); // ensure unique material
        spriteRenderer.material = material;

        playerReferences = GetComponent<PlayerReferences>();
    }

    void Update()
    {
        if (playerReferences == null) return;

        Vector2 velocity = playerReferences.PRB.linearVelocity;

        // Set noise scroll direction (based on velocity)
        float noiseX = Mathf.Abs(velocity.x) * horizontalNoiseSpeedScale;
        float noiseY = -velocity.y * verticalNoiseSpeedScale;
        material.SetVector(NoiseSpeedID, new Vector2(noiseX, noiseY));

        // Calculate dynamic scale multiplier
        float speedMagnitude = velocity.magnitude;
        float scaleFactor = 1f + (scaleSensitivity * speedMagnitude);

        if (!scaleIncreasesWithSpeed)
        {
            // Avoid zero or negative scale
            scaleFactor = Mathf.Max(0.1f, 1f - (scaleSensitivity * speedMagnitude));
        }

        // Apply scale factor
        Vector2 scaledNoise = baseNoiseScale * scaleFactor;
        material.SetVector(NoiseScaleID, scaledNoise);
    }
}
