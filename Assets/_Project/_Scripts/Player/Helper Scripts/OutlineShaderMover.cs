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

    [Header("Noise Scale - X (Left/Right)")]
    public float xScaleSensitivity = 0.5f;
    public bool xScaleIncreasesWithSpeed = true;

    [Header("Noise Scale - Y (Up/Down)")]
    public float yScaleSensitivity = 0.5f;
    public bool yScaleIncreasesWithSpeed = true;

    private SpriteRenderer spriteRenderer;
    private Material material;
    private PlayerReferences playerReferences;

    private static readonly int NoiseSpeedID = Shader.PropertyToID("_OuterOutlineNoiseSpeed");
    private static readonly int NoiseScaleID = Shader.PropertyToID("_OuterOutlineNoiseScale");

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = new Material(spriteRenderer.material); // Create instance
        spriteRenderer.material = material;

        playerReferences = GetComponent<PlayerReferences>();
    }

    void Update()
    {
        if (playerReferences == null) return;

        Vector2 velocity = playerReferences.PRB.linearVelocity;

        // === Noise SPEED ===
        float noiseX = Mathf.Abs(velocity.x) * horizontalNoiseSpeedScale;
        float noiseY = velocity.y * verticalNoiseSpeedScale;
        material.SetVector(NoiseSpeedID, new Vector2(noiseX, noiseY));

        // === Noise SCALE ===
        float xSpeed = Mathf.Abs(velocity.x);
        float ySpeed = Mathf.Abs(velocity.y);

        float scaleX = baseScaleX;
        float scaleY = baseScaleY;

        // Modify X scale based on X movement
        if (xScaleIncreasesWithSpeed)
        {
            scaleX += xSpeed * xScaleSensitivity;
        }
        else
        {
            scaleX = Mathf.Max(0.05f, baseScaleX - xSpeed * xScaleSensitivity);
        }

        // Modify Y scale based on Y movement
        if (yScaleIncreasesWithSpeed)
        {
            scaleY += ySpeed * yScaleSensitivity;
        }
        else
        {
            scaleY = Mathf.Max(0.05f, baseScaleY - ySpeed * yScaleSensitivity);
        }

        material.SetVector(NoiseScaleID, new Vector2(scaleX, scaleY));
    }
}
