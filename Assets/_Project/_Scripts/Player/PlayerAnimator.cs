using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    // Cached parameter hashes for performance (optional enhancement)
    private int isGroundedHash = Animator.StringToHash("IsGrounded");
    private int speedHash = Animator.StringToHash("Speed");
    private int jumpTriggerHash = Animator.StringToHash("Jump");
    private int fallTriggerHash = Animator.StringToHash("Fall");
    private int landTriggerHash = Animator.StringToHash("Land");

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    // ======== Basic Movement ========

    public void SetIsGrounded(bool value)
    {
        animator.SetBool(isGroundedHash, value);
    }

    public void SetSpeed(float value)
    {
        animator.SetFloat(speedHash, value);
    }

    // ======== Action Triggers ========

    public void PlayJump()
    {
        animator.SetTrigger(jumpTriggerHash);
    }

    public void PlayFall()
    {
        animator.SetTrigger(fallTriggerHash);
    }

    public void PlayLand()
    {
        animator.SetTrigger(landTriggerHash);
    }

    // ======== Future Extensions (placeholders) ========

    public void PlayDash() 
    {
        // animator.SetTrigger("Dash");
    }

    public void SetClimbState(bool value)
    {
        // animator.SetBool("IsClimbing", value);
    }

    public void PlayLedgeGrab()
    {
        // animator.SetTrigger("LedgeGrab");
    }

    public void PlayWallSlide(bool isSliding)
    {
        // animator.SetBool("IsWallSliding", isSliding);
    }

    public void SetLookDirection(Vector2 direction)
    {
        // for aiming, 8-way animations, etc.
    }

    public void OnAnimationEvent(string eventName)
    {
        // Hook animation event names to custom callbacks
    }

    public void ResetAllTriggers()
    {
        // Optional: reset any triggers to prevent state leaks
        // animator.ResetTrigger(jumpTriggerHash);
        // animator.ResetTrigger(landTriggerHash);
    }
}