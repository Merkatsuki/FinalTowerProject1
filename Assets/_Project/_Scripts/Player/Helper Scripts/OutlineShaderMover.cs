using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class OutlineShaderMover : MonoBehaviour
{
    public float noiseSpeedMultiplier = 1.0f; // Adjust this to control the effect's intensity

    private SpriteRenderer spriteRenderer;
    private Material material;
    private Vector3 previousPosition;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
        previousPosition = transform.position;
    }

    void Update()
    {
        Vector3 currentPosition = transform.position;
        Vector3 movement = currentPosition - previousPosition;

        // Calculate movement direction
        Vector2 movementDirection = new Vector2(movement.x, movement.y).normalized;

        // Scale the movement direction
        Vector2 noiseSpeed = movementDirection * noiseSpeedMultiplier;

        // Update the shader property
        material.SetVector("_OuterOutlineNoiseSpeed", noiseSpeed);

        previousPosition = currentPosition;
    }
}
