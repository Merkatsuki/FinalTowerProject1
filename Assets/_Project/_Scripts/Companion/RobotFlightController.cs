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
    public float bobFrequency = 2f;
    public float bobAmplitude = 0.15f;
    public float swayAmplitude = 0.1f;
    public float obstacleCheckDistance = 1f;
    public LayerMask obstacleMask;
    public float idleThreshold = 0.05f;

    private Rigidbody2D rb;
    private Vector2 velocity;
    private Vector2 targetPosition;
    private bool hasTarget = false;

    private float bobOffset;
    private float swayOffset;

    public bool IsHovering => velocity.magnitude < idleThreshold && !hasTarget;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bobOffset = Random.Range(0f, 2f * Mathf.PI);
        swayOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    private void FixedUpdate()
    {
        if (!hasTarget) return;

        Vector2 position = rb.position;
        Vector2 desiredDirection = (targetPosition - position).normalized;
        float distanceToTarget = Vector2.Distance(position, targetPosition);

        Vector2 adjustedTarget = targetPosition;
        if (adjustedTarget.y < minHeight)
            adjustedTarget.y = minHeight;

        RaycastHit2D hit = Physics2D.Raycast(position, desiredDirection, obstacleCheckDistance, obstacleMask);
        if (hit.collider != null)
        {
            Vector2 obstacleNormal = hit.normal;
            desiredDirection = Vector2.Reflect(desiredDirection, obstacleNormal).normalized;
            adjustedTarget = position + desiredDirection * 0.5f;
        }

        float bob = Mathf.Sin(Time.time * bobFrequency + bobOffset) * bobAmplitude;
        float sway = Mathf.Cos(Time.time * bobFrequency + swayOffset) * swayAmplitude;
        Vector2 motionOffset = new Vector2(sway, bob);

        Vector2 finalTarget = adjustedTarget + motionOffset;

        float speedFactor = Mathf.Clamp01(distanceToTarget / decelerationRadius);
        Vector2 desiredVelocity = (finalTarget - position).normalized * speed * speedFactor;

        velocity = Vector2.Lerp(velocity, desiredVelocity, acceleration * Time.fixedDeltaTime);
        rb.MovePosition(position + velocity * Time.fixedDeltaTime);
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

        if (rb != null && hasTarget)
        {
            Gizmos.color = Color.red;
            Vector2 direction = (targetPosition - rb.position).normalized;
            Gizmos.DrawLine(rb.position, rb.position + direction * obstacleCheckDistance);
        }
    }
#endif
}