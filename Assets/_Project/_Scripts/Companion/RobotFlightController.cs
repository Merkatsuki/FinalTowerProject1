using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RobotFlightController : MonoBehaviour
{
    [Header("Flight Movement")]
    public float speed = 3f;
    public float acceleration = 6f;
    public float decelerationRadius = 1.5f;
    public float minHeight = 1.5f;
    public LayerMask obstacleMask;
    public float idleThreshold = 0.05f;
    public Transform defaultFollowTarget;

    [Header("Avoidance Settings")]
    public float avoidanceWeight = 0.5f;
    public int rayCount = 9;
    public float rayArcAngle = 90f;
    public float rayLength = 1f;

    [Header("Visual References")]
    public Transform visualRoot;
    public ParticleSystem thrustParticles;
    public float tiltMultiplier = 10f;
    public float tiltSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 velocity;
    private Vector2 lastVelocity;
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

        if (!hasTarget && defaultFollowTarget != null)
        {
            SetTarget(defaultFollowTarget.position);
        }

        Vector2 desiredDirection = (targetPosition - position);
        float distanceToTarget = desiredDirection.magnitude;
        desiredDirection = desiredDirection.normalized;

        // Multi-ray arc obstacle avoidance
        Vector2 avoidance = Vector2.zero;
        float startAngle = -rayArcAngle * 0.5f;
        float angleStep = rayArcAngle / (rayCount - 1);

        for (int i = 0; i < rayCount; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector2 rayDir = Quaternion.Euler(0, 0, angle) * desiredDirection;
            RaycastHit2D hit = Physics2D.Raycast(position, rayDir, rayLength, obstacleMask);
            if (hit.collider != null)
            {
                float distanceFactor = 1f - (hit.distance / rayLength);
                avoidance -= rayDir.normalized * distanceFactor;
            }
        }

        if (avoidance != Vector2.zero)
        {
            desiredDirection = Vector2.Lerp(desiredDirection, desiredDirection + avoidance.normalized, avoidanceWeight).normalized;
        }

        float speedFactor = Mathf.Clamp01(distanceToTarget / decelerationRadius);
        Vector2 desiredVelocity = desiredDirection * speed * speedFactor;

        lastVelocity = velocity;
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

            Vector2 accelerationVector = (velocity - lastVelocity) / Time.fixedDeltaTime;
            float tiltZ = Mathf.Clamp(-accelerationVector.x * tiltMultiplier, -tiltMultiplier, tiltMultiplier);
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, tiltZ);
            visualRoot.localRotation = Quaternion.Slerp(visualRoot.localRotation, targetRotation, tiltSpeed * Time.deltaTime);
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

        if (rb != null && Application.isPlaying)
        {
            Vector2 position = rb.position;
            Vector2 forward = (targetPosition - position).normalized;
            float startAngle = -rayArcAngle * 0.5f;
            float angleStep = rayArcAngle / (rayCount - 1);

            for (int i = 0; i < rayCount; i++)
            {
                float angle = startAngle + angleStep * i;
                Vector2 rayDir = Quaternion.Euler(0, 0, angle) * forward;
                Gizmos.color = Color.red;
                Gizmos.DrawRay(position, rayDir * rayLength);
            }
        }
    }
#endif
}
