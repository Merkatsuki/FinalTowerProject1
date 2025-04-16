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

    // Debug hover sampling state
    private Vector2[] debugSamplePoints;
    private Vector2 debugFallbackPoint;

    public bool IsHovering => velocity.magnitude < idleThreshold && !hasTarget;

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    private void FixedUpdate()
    {
        if (!hasTarget && defaultFollowTarget != null)
        {
            SetTarget(defaultFollowTarget.position);
        }

        Vector2 position = rb.position;
        Vector2 desiredDirection = CalculateDesiredDirection(position);
        float distanceToTarget = Vector2.Distance(position, targetPosition);

        Vector2 desiredVelocity = CalculateVelocity(desiredDirection, distanceToTarget);
        ApplyVelocity(position, desiredVelocity);
        UpdateVisuals(velocity);
    }

    private Vector2 CalculateDesiredDirection(Vector2 position)
    {
        Vector2 direction = (targetPosition - position).normalized;
        Vector2 avoidance = CalculateAvoidanceVector(position, direction);
        return avoidance != Vector2.zero
            ? Vector2.Lerp(direction, direction + avoidance.normalized, avoidanceWeight).normalized
            : direction;
    }

    private Vector2 CalculateAvoidanceVector(Vector2 position, Vector2 forward)
    {
        Vector2 avoidance = Vector2.zero;
        float startAngle = -rayArcAngle * 0.5f;
        float angleStep = rayArcAngle / (rayCount - 1);

        for (int i = 0; i < rayCount; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector2 rayDir = Quaternion.Euler(0, 0, angle) * forward;
            RaycastHit2D hit = Physics2D.Raycast(position, rayDir, rayLength, obstacleMask);
            if (hit.collider != null)
            {
                float factor = 1f - (hit.distance / rayLength);
                avoidance -= rayDir.normalized * factor;
            }
        }

        return avoidance;
    }

    private Vector2 CalculateVelocity(Vector2 direction, float distanceToTarget)
    {
        float speedFactor = Mathf.Clamp01(distanceToTarget / decelerationRadius);
        Vector2 desiredVelocity = direction * speed * speedFactor;
        lastVelocity = velocity;
        velocity = Vector2.Lerp(velocity, desiredVelocity, acceleration * Time.fixedDeltaTime);
        return velocity;
    }

    private void ApplyVelocity(Vector2 position, Vector2 velocity)
    {
        rb.MovePosition(position + velocity * Time.fixedDeltaTime);
    }

    public void SetTarget(Vector2 position)
    {
        targetPosition = position;
        hasTarget = true;
    }

    public void SetTargetWithHoverProfile(Vector2 targetPos, HoverStagingProfileSO profile)
    {
        Vector2 offset = profile.mode switch
        {
            HoverMode.FixedDirection => profile.fixedDirection.normalized * profile.offsetRadius,
            HoverMode.RelativeToApproach => (rb.position - targetPos).normalized * profile.offsetRadius,
            HoverMode.SmartSampled => SampleSmartHoverOffset(targetPos, profile, out bool usedFallback),
            _ => Vector2.zero
        };

        SetTarget(targetPos + offset);
    }

    private Vector2 SampleSmartHoverOffset(Vector2 targetPos, HoverStagingProfileSO profile, out bool usedFallback)
    {
        usedFallback = false;
        Vector2 bestOffset = Vector2.zero;
        float bestClearance = 0f;

        float angleStep = profile.sampleArcDegrees / (profile.sampleRayCount - 1);
        float startAngle = -profile.sampleArcDegrees / 2f;

        debugSamplePoints = new Vector2[profile.sampleRayCount];

        for (int i = 0; i < profile.sampleRayCount; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.up;
            Vector2 candidate = targetPos + dir * profile.offsetRadius;
            debugSamplePoints[i] = candidate;

            if (!Physics2D.OverlapCircle(candidate, 0.2f, profile.obstacleMask))
            {
                float clearance = Vector2.Distance(rb.position, candidate);
                if (clearance > bestClearance)
                {
                    bestClearance = clearance;
                    bestOffset = dir * profile.offsetRadius;
                }
            }
        }

        if (bestClearance <= 0.01f)
        {
            usedFallback = true;
            debugFallbackPoint = (rb.position - targetPos).normalized * profile.offsetRadius;
            return debugFallbackPoint;
        }

        debugFallbackPoint = Vector2.zero;
        return bestOffset;
    }

    private void UpdateVisuals(Vector2 currentVelocity)
    {
        if (visualRoot != null)
        {
            UpdateVisualTilt(currentVelocity);
            UpdateVisualScale(currentVelocity);
        }

        if (thrustParticles != null)
        {
            var emission = thrustParticles.emission;
            emission.rateOverTime = Mathf.Lerp(2f, 30f, currentVelocity.magnitude / speed);
        }
    }

    private void UpdateVisualTilt(Vector2 currentVelocity)
    {
        Vector2 accelerationVector = (velocity - lastVelocity) / Time.fixedDeltaTime;
        float tiltZ = Mathf.Clamp(-accelerationVector.x * tiltMultiplier, -tiltMultiplier, tiltMultiplier);
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, tiltZ);
        visualRoot.localRotation = Quaternion.Slerp(visualRoot.localRotation, targetRotation, tiltSpeed * Time.deltaTime);
    }

    private void UpdateVisualScale(Vector2 currentVelocity)
    {
        if (currentVelocity.magnitude > 0.1f)
        {
            visualRoot.localScale = new Vector3(
                Mathf.Sign(currentVelocity.x),
                visualRoot.localScale.y,
                visualRoot.localScale.z);
        }
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

            if (debugSamplePoints != null)
            {
                foreach (var point in debugSamplePoints)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(point, 0.1f);
                }

                if (debugFallbackPoint != Vector2.zero)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(rb.position + debugFallbackPoint, 0.15f);
                }
            }
        }
    }
#endif
}
