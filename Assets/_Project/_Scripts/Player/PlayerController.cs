using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float jumpForce = 11f;
    public Vector2 wallJumpDirection = new Vector2(2f, 4f);
    public float wallJumpMultiplier = 15f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public Transform wallCheckFront;
    public Transform wallCheckBack;
    public float wallCheckDistance = 0.1f;
    public LayerMask groundLayer;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.15f;

    public Rigidbody2D rb;
    [SerializeField] public Animator animator;

    private Vector3 originalScale;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private bool preserveMomentum = false;

    public bool IsFacingRight => transform.localScale.x > 0;
    public bool JumpBuffered => jumpBufferTimer > 0;
    public bool CanCoyoteJump => coyoteTimer > 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (IsGrounded())
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;

        jumpBufferTimer -= Time.deltaTime;

        //Debug.Log($"[PLAYER] CoyoteTimer: {coyoteTimer:F2} | JumpBuffer: {jumpBufferTimer:F2}");
    }

    public void BufferJump()
    {
        jumpBufferTimer = jumpBufferTime;
    }

    public void Move(Vector2 input, bool applyMovement = true)
    {
        if (!applyMovement)
        {
            return;
        }

        if (!preserveMomentum)
        {
            rb.linearVelocity = new Vector2(input.x * moveSpeed, rb.linearVelocity.y);

            if (input.x > 0 && !IsFacingRight)
                Flip();
            else if (input.x < 0 && IsFacingRight)
                Flip();
        }

        animator?.SetFloat("Speed", Mathf.Abs(input.x));
        animator?.SetBool("IsGrounded", IsGrounded());
    }

    public void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        animator?.SetTrigger("Jump");
        preserveMomentum = false;
        Debug.Log("[PLAYER] Jump Force Applied");
    }

    public void WallJump()
    {
        Vector2 dir = new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpDirection.x, wallJumpDirection.y).normalized;
        rb.AddForce(dir * wallJumpMultiplier, ForceMode2D.Impulse);
        preserveMomentum = true;
        Flip();
        animator?.SetTrigger("Jump");
        Debug.Log($"[PLAYER] Wall Jump Force Applied: {dir * wallJumpMultiplier}");
    }

    public void StopMovement()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        animator?.SetFloat("Speed", 0);
    }

    public bool IsGrounded()
    {
        if (groundCheck == null) return true;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public bool IsTouchingFrontWall()
    {
        if (wallCheckFront == null) return false;
        return Physics2D.Raycast(wallCheckFront.position, IsFacingRight ? Vector2.right : Vector2.left, wallCheckDistance, groundLayer);
    }

    public bool IsTouchingBackWall()
    {
        if (wallCheckBack == null) return false;
        return Physics2D.Raycast(wallCheckBack.position, IsFacingRight ? Vector2.left : Vector2.right, wallCheckDistance, groundLayer);
    }

    public void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.color = Color.red;
        if (wallCheckFront != null)
            Gizmos.DrawLine(wallCheckFront.position, wallCheckFront.position + (IsFacingRight ? Vector3.right : Vector3.left) * wallCheckDistance);
        if (wallCheckBack != null)
            Gizmos.DrawLine(wallCheckBack.position, wallCheckBack.position + (IsFacingRight ? Vector3.left : Vector3.right) * wallCheckDistance);
    }

    public void ResetMomentumPreservation()
    {
        preserveMomentum = false;
    }
}