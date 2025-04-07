
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInputActions inputActions;

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
    }

    public Vector2 GetMoveInput() => inputActions.Player.Move.ReadValue<Vector2>();
    public bool GetJumpPressed() => inputActions.Player.Jump.triggered;
    public bool GetInteractPressed() => inputActions.Player.Interact.triggered;
}