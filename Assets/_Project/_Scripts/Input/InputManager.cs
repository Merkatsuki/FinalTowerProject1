
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInputActions inputActions;

    private Vector2 moveInput;
    private bool isJumpPressed = false;
    private bool wasJumpPressedThisFrame = false;

    public Vector2 MoveInput => moveInput;
    public bool IsJumpPressed => isJumpPressed;
    public bool WasJumpPressedThisFrame => wasJumpPressedThisFrame;

    public static event System.Action<Vector2> OnMove;
    public static event System.Action OnJumpRequested;
    public static event System.Action OnInteractPressed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        inputActions = new PlayerInputActions();
        inputActions.Enable();

        inputActions.Player.Move.performed += ctx => {
            Vector2 rawInput = ctx.ReadValue<Vector2>();
            moveInput = new Vector2(Mathf.Round(rawInput.x), Mathf.Round(rawInput.y));
            OnMove?.Invoke(moveInput);
        };

        inputActions.Player.Move.canceled += ctx => {
            moveInput = Vector2.zero;
            OnMove?.Invoke(moveInput);
        };

        inputActions.Player.Jump.started += ctx => {
            isJumpPressed = true;
            wasJumpPressedThisFrame = true;
            OnJumpRequested?.Invoke(); // New!
        };

        inputActions.Player.Jump.canceled += ctx => {
            isJumpPressed = false;
        };

        inputActions.Player.Interact.performed += ctx => OnInteractPressed?.Invoke();
    }

    public bool IsJumpHeld => inputActions.Player.Jump.IsPressed();

    private void Update()
    {
        wasJumpPressedThisFrame = false;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}