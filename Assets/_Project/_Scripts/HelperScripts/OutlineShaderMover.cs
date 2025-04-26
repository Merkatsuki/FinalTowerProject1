using UnityEngine;
using Momentum;

[RequireComponent(typeof(SpriteRenderer))]
public class OutlineShaderMover : MonoBehaviour
{
    [Header("Noise Scroll Speed")]
    public float horizontalNoiseSpeedScale = 0.05f;
    public float verticalNoiseSpeedScale = 0.05f;

    [Header("Base Noise Scale")]
    public float baseScaleX = 1f;
    public float baseScaleY = 1f;

    [Header("Noise Scale Curves")]
    [Tooltip("Controls how noise scale X changes based on horizontal speed.")]
    public AnimationCurve xScaleCurve = AnimationCurve.Linear(0, 1, 10, 2);

    [Tooltip("Controls how noise scale Y changes based on vertical speed.")]
    public AnimationCurve yScaleCurve = AnimationCurve.Linear(0, 1, 10, 2);

    // --- Private references
    private SpriteRenderer spriteRenderer;
    private Material material;
    private PlayerReferences playerReferences;

    // --- Shader Property IDs
    private static readonly int NoiseSpeedID = Shader.PropertyToID("_OuterOutlineNoiseSpeed");
    private static readonly int NoiseScaleID = Shader.PropertyToID("_OuterOutlineNoiseScale");

    // --- Burst System (not active yet)
    private Vector2 burstScaleOffset = Vector2.zero;
    private float burstTimer = 0f;
    private float burstDuration = 0f;

    void Start()
    {
        InitializeReferences();
    }

    void Update()
    {
        if (playerReferences == null) return;

        Vector2 velocity = playerReferences.PRB.linearVelocity;

        UpdateNoiseSpeed(velocity);
        UpdateNoiseScale(velocity);
        UpdateBurstTimer();

        ApplyShaderValues();
    }

    // -------------------------
    // Initialization
    // -------------------------
    private void InitializeReferences()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = new Material(spriteRenderer.material); // ensure unique material
        spriteRenderer.material = material;

        playerReferences = GetComponent<PlayerReferences>();
    }

    // -------------------------
    // Noise Scroll Speed
    // -------------------------
    private void UpdateNoiseSpeed(Vector2 velocity)
    {
        float noiseX = Mathf.Abs(velocity.x) * horizontalNoiseSpeedScale;
        float noiseY = velocity.y * verticalNoiseSpeedScale;

        material.SetVector(NoiseSpeedID, new Vector2(noiseX, noiseY));
    }

    // -------------------------
    // Noise Scale (Curve + Burst)
    // -------------------------
    private void UpdateNoiseScale(Vector2 velocity)
    {
        float scaleX = baseScaleX * xScaleCurve.Evaluate(Mathf.Abs(velocity.x));
        float scaleY = baseScaleY * yScaleCurve.Evaluate(Mathf.Abs(velocity.y));

        // Add burst offset if any
        Vector2 finalScale = new Vector2(scaleX, scaleY) + burstScaleOffset;

        material.SetVector(NoiseScaleID, finalScale);
    }

    // -------------------------
    // Burst System (Prep Only)
    // -------------------------
    private void UpdateBurstTimer()
    {
        if (burstTimer > 0f)
        {
            burstTimer -= Time.deltaTime;
            if (burstTimer <= 0f)
            {
                burstScaleOffset = Vector2.zero;
            }
        }
    }

    public void TriggerNoiseBurst(Vector2 burstAmount, float duration)
    {
        burstScaleOffset = burstAmount;
        burstDuration = duration;
        burstTimer = duration;
    }

    // -------------------------
    // Apply to Shader
    // -------------------------
    private void ApplyShaderValues()
    {
        // Speed and scale are already applied inside the update methods
        // This is here if additional properties need to be batched later
    }
}
