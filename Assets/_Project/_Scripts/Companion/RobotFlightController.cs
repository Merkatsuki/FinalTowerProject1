// RobotFlightController.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RobotFlightController : MonoBehaviour
{
    [Header("Flight Movement")]
    public float speed = 3f;
    public float acceleration = 6f;
    public float decelerationRadius = 1.5f;
    public float minHeight = 1.5f;
    public float obstacleCheckDistance = 1f;
    public LayerMask obstacleMask;
    public float idleThreshold = 0.05f;
    public Transform defaultFollowTarget;
    public float avoidanceWeight = 0.5f;
    public float avoidanceRadius = 1f;

    [Header("Visual References")]
    public Transform visualRoot;
    public ParticleSystem thrustParticles;

    private Rigidbody2D rb;
    private Vector2 velocity;
    private Vector2 targetPosition;
    private bool hasTarget = false;

    public bool IsHovering => velocity.magnitude < idleThreshold && !hasTarget;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector2 position = rb.position;

        // Default follow target when idle
        if (!hasTarget && defaultFollowTarget != null)
        {
            SetTarget(defaultFollowTarget.position);
        }

        Vector2 desiredDirection = (targetPosition - position);
        float distanceToTarget = desiredDirection.magnitude;

        // Base direction
        desiredDirection = desiredDirection.normalized;

        Vector2 adjustedTarget = targetPosition;

        // Adaptive obstacle avoidance
        Vector2 avoidance = Vector2.zero;
        Vector2[] directions = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        foreach (var dir in directions)
        {
            RaycastHit2D hit = Physics2D.Raycast(position, dir, avoidanceRadius, obstacleMask);
            if (hit.collider != null)
            {
                avoidance -= dir; // Steer away from obstacle
            }
        }

        if (avoidance != Vector2.zero)
        {
            desiredDirection = Vector2.Lerp(desiredDirection, desiredDirection + avoidance.normalized, avoidanceWeight).normalized;
        }

        float speedFactor = Mathf.Clamp01(distanceToTarget / decelerationRadius);
        Vector2 desiredVelocity = desiredDirection * speed * speedFactor;

        velocity = Vector2.Lerp(velocity, desiredVelocity, acceleration * Time.fixedDeltaTime);
        rb.MovePosition(position + velocity * Time.fixedDeltaTime);

        UpdateVisuals(velocity);
    }

    private void UpdateVisuals(Vector2 currentVelocity)
    {
        if (visualRoot != null)
        {
            if (currentVelocity.magnitude > 0.1f)
            {
                visualRoot.localScale = new Vector3(
                    Mathf.Sign(currentVelocity.x),
                    visualRoot.localScale.y,
                    visualRoot.localScale.z);
            }
        }

        if (thrustParticles != null)
        {
            var emission = thrustParticles.emission;
            emission.rateOverTime = Mathf.Lerp(2f, 30f, currentVelocity.magnitude / speed);
        }
    }

    public void SetTarget(Vector2 position)
    {
        targetPosition = position;
        hasTarget = true;
    }

    public void ClearTarget()
    {
        hasTarget = false;
        velocity = Vector2.zero;
    }

    public bool ReachedTarget(float threshold = 0.1f)
    {
        return Vector2.Distance(rb.position, targetPosition) <= threshold;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(targetPosition, 0.1f);

        if (rb != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(rb.position, avoidanceRadius);
        }
    }
#endif
}
