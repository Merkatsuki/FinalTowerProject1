using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WindZone2D : MonoBehaviour
{
    public Vector2 windDirection = Vector2.right;
    public float force = 5f;
    public bool isPulsing = false;
    public float pulseInterval = 2f;

    private float pulseTimer = 0f;

    void FixedUpdate()
    {
        if (isPulsing)
        {
            pulseTimer += Time.fixedDeltaTime;
            if (pulseTimer >= pulseInterval)
                pulseTimer = 0f;
        }
    }

    public bool IsActive()
    {
        return !isPulsing || pulseTimer <= pulseInterval / 2f;
    }

    public Vector2 GetWindForce()
    {
        return IsActive() ? windDirection.normalized * force : Vector2.zero;
    }
}
