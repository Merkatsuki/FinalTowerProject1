using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerStateMachine : StateMachine<PlayerState>, IGameStateListener
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded = true; // Simplified for now
    private float horizontalInput;

    protected override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        GameStateManager.Instance?.RegisterListener(this);
        ChangeState(PlayerState.Idle);
    }

    private void OnDestroy()
    {
        GameStateManager.Instance?.UnregisterListener(this);
    }

    private void Update()
    {
        if (GameStateManager.Instance == null || !GameStateManager.Instance.Is(GameState.Gameplay))
            return;

        HandleInput();
        UpdateState();
    }

    private void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            ChangeState(PlayerState.Jumping);
        }
        else if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            ChangeState(PlayerState.Walking);
        }
        else
        {
            ChangeState(PlayerState.Idle);
        }
    }

    private void UpdateState()
    {
        switch (CurrentState)
        {
            case PlayerState.Walking:
                rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
                break;
            case PlayerState.Jumping:
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                isGrounded = false; // You'd update this based on collision logic
                ChangeState(PlayerState.Idle); // Simplified transition
                break;
            case PlayerState.Idle:
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                break;
            case PlayerState.Interacting:
            case PlayerState.Frozen:
                rb.linearVelocity = Vector2.zero;
                break;
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
            animator.SetBool("IsGrounded", isGrounded);
        }
    }

    public void OnGameStateChanged(GameState newState)
    {
        if (newState != GameState.Gameplay)
            ChangeState(PlayerState.Frozen);
    }
}
