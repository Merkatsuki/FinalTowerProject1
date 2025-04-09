using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [Range(1f, 20f)] public float maxSpeed = 7f;
    [Range(10f, 200f)] public float acceleration = 70f;
    [Range(10f, 200f)] public float deceleration = 100f;
    [Range(10f, 200f)] public float airAcceleration = 50f;
    [Range(10f, 200f)] public float airDeceleration = 60f;

    [Header("Jumping")]
    [Range(5f, 45f)] public float jumpForce = 25f;
    [Range(0f, 10f)] public float gravityScale = 5f;
    [Range(0f, 100f)] public float maxFallSpeed = 20f;
    [Range(1f, 5f)] public float jumpCutMultiplier = 2f;
    [Range(1f, 5f)] public float fallMultiplier = 2.5f;
    [Range(0f, 0.5f)] public float coyoteTime = 0.15f;
    [Range(0f, 0.5f)] public float jumpBufferTime = 0.15f;

    [Header("Grounding")]
    [Range(0f, 1f)] public float groundCheckRadius = 0.2f;
    public Transform groundCheck;
    public Transform wallCheckFront;
    public Transform wallCheckBack;
    public LayerMask groundLayer;
    [Range(-20f, 0f)] public float groundingForce = -5f;

    [Header("Input")]
    [Range(0f, 1f)] public float inputDeadzone = 0.2f;
    public bool snapInput = true;

    public Rigidbody2D rb;
    public Animator animator;

    private bool isJumpCut;
    private bool preserveMomentum;
    private Vector3 originalScale;

    public float lastGroundedTime { get; private set; }
    public float lastJumpInputTime { get; private set; }
    
    public bool IsFacingRight => transform.localScale.x > 0;
    public Vector2 LastInput { get; private set; }
    public bool WasJumpPressedThisFrame => InputManager.Instance.WasJumpPressedThisFrame;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
    }

    private void Update()
    {
        lastGroundedTime -= Time.deltaTime;

        if (IsGrounded())
            lastGroundedTime = coyoteTime;

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
        {
            isJumpCut = true;
        }

        animator?.SetBool("IsGrounded", IsGrounded());
        animator?.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }

    private void FixedUpdate()
    {
        HandleGravity();

        if (IsGrounded() && rb.linearVelocity.y < 0)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, groundingForce);
    }

    private void OnEnable()
    {
        InputManager.OnJumpRequested += BufferJump;
    }

    private void OnDisable()
    {
        InputManager.OnJumpRequested -= BufferJump;
    }

    public void Move(Vector2 input, bool applyMovement = true)
    {
        if (!applyMovement || preserveMomentum) return;

        float targetSpeed = input.x * maxSpeed;
        float accel = IsGrounded()
            ? (Mathf.Abs(input.x) > 0.01f ? acceleration : deceleration)
            : (Mathf.Abs(input.x) > 0.01f ? airAcceleration : airDeceleration);

        float newX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, accel * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);

        if (input.x > 0 && !IsFacingRight) Flip();
        else if (input.x < 0 && IsFacingRight) Flip();
    }

    public void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        lastGroundedTime = 0f;
        isJumpCut = !InputManager.Instance.IsJumpHeld;

        preserveMomentum = false;
        animator?.SetTrigger("Jump");
        ClearJumpBuffer();
    }

    public void BufferJump() => lastJumpInputTime = Time.time;
    public bool HasBufferedJump() => Time.time - lastJumpInputTime <= jumpBufferTime;
    public void ClearJumpBuffer() => lastJumpInputTime = -Mathf.Infinity;
    public bool IsGrounded() => Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

    public bool IsTouchingFrontWall()
    {
        return wallCheckFront && Physics2D.Raycast(wallCheckFront.position, IsFacingRight ? Vector2.right : Vector2.left, 0.1f, groundLayer);
    }

    public bool IsTouchingBackWall()
    {
        return wallCheckBack && Physics2D.Raycast(wallCheckBack.position, IsFacingRight ? Vector2.left : Vector2.right, 0.1f, groundLayer);
    }

    public void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, originalScale.y, originalScale.z);
    }

    private void HandleGravity()
    {
        float gravity = gravityScale;
        if (rb.linearVelocity.y < 0)
            gravity *= fallMultiplier;
        else if (isJumpCut)
            gravity *= jumpCutMultiplier;

        rb.linearVelocity += Vector2.up * Physics2D.gravity.y * gravity * Time.fixedDeltaTime;

        // Clamp max fall speed
        if (rb.linearVelocity.y < -maxFallSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxFallSpeed);
        }
    }

    public void StopMovement()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        StopMovementAnimation();
    }

    public void StopMovementAnimation()
    {
        animator?.SetFloat("Speed", 0f);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
