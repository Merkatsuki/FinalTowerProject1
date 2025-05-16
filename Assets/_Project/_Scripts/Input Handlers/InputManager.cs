using System;
using UnityEngine;

namespace Momentum
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager instance; // Creates one instance for everyone to use
        private InputMaster _controls; // Reference to the InputMaster script where our controls are stored

        private bool _isDialogueMode = false;
        
        #region Player Input Map
        // Avalible for the rest of the classes to use (Mainly should be used by PlayerAction as a front end for other classes)

        private Vector2 previousWASDInput;
        public Vector2 WASDInput { get; private set; }
        public Vector2 MousePosition { get; private set; }

        [NonSerialized] public Action JumpPressed; // Public so other classes can subscribe to this. Tagged to not show up in inspector
        [NonSerialized] public Action JumpReleased;
        public event Action OnMoveCommand;
        public event Action<bool> OnCommandModeChanged;

        private bool _jumpBlocked = false;
        public bool JumpBlocked => _jumpBlocked;
        public bool JumpPressedThisFrame => !_jumpBlocked && _controls.Player.Jump.WasPressedThisFrame();
        public bool JumpHeld => _controls.Player.Jump.inProgress && !_jumpBlocked;
        public bool IsDialogueMode => _isDialogueMode;

        public bool IsPressingDown => WASDInput.y < -0.5f;
        public bool IsSprint { get; private set; }
        public bool IsDash { get; private set; }
        public bool IsCrouch { get; private set; }
        public bool IsGrab { get; private set; }
        public bool IsAttack { get; private set; }
        public bool IsCommandMode { get; private set; }

        #endregion

        private void Awake()
        {
            // The Singleton Pattern makes sure there is only one instance of this class
            #region Singleton

            if (instance == null) // If no instance has been created
            {
                instance = this; // Make the instance
                _controls = new InputMaster();
                DontDestroyOnLoad(gameObject); // Make sure object this script is attached to is not destoryed when scene is switched
            }
            else // If there is an instance existing
            {
                Destroy(gameObject); // Get rid of it
                return; // Close method
            }

            #endregion

            _controls.Player.CommandMode.performed += ctx => ToggleCommandMode();

            _controls.Player.Jump.performed += ctx =>
            {
                if (!_jumpBlocked)
                {
                    JumpPressed?.Invoke();
                }
            };
        }

        private void Update()
        {
            #region Read Input
            if (_isDialogueMode)
            {
                WASDInput = Vector2.zero;
                IsSprint = false;
                IsDash = false;
                IsCrouch = false;
                IsGrab = false;
                IsAttack = false;
                return;
            }

            MousePosition = _controls.Player.PointerPosition.ReadValue<Vector2>();
            WASDInput = _controls.Player.MovementControls.ReadValue<Vector2>();

            if (WASDInput != Vector2.zero && previousWASDInput == Vector2.zero)
            {
                OnMoveCommand?.Invoke();
            }

            previousWASDInput = WASDInput;

            if (_controls.Player.Jump.WasPressedThisFrame() && !_jumpBlocked)
            {
                JumpPressed?.Invoke();
            }

            if (_controls.Player.Jump.WasReleasedThisFrame())
            {
                JumpReleased?.Invoke();
                _jumpBlocked = false;
            }

            // Set bools
            if (_controls.Player.Sprint.IsPressed()) { IsSprint = true; } else if (!_controls.Player.Sprint.IsPressed()) { IsSprint = false; }
            if (_controls.Player.Dash.IsPressed()) { IsDash = true; } else if (!_controls.Player.Dash.IsPressed()) { IsDash = false; }
            if (_controls.Player.Crouch.IsPressed()) { IsCrouch = true; } else if (!_controls.Player.Crouch.IsPressed()) { IsCrouch = false; }
            if (_controls.Player.Grab.IsPressed()) { IsGrab = true; } else if (!_controls.Player.Grab.IsPressed()) { IsGrab = false; }
            #endregion
        }

        #region Enable/Disable Input

        private void OnEnable() // Disable and enable the controls key mappings when parent gameobject is disabled or enabled to prevent errors
        {
            _controls.Enable();
        }

        private void OnDisable()
        {
            _controls.Disable();
        }

        private void ToggleCommandMode()
        {
            var unlockFlag = FlagManager.Instance.CommandModeUnlockedFlag;
            if (!FlagManager.Instance.IsFlagSet(unlockFlag)) return;
            IsCommandMode = !IsCommandMode;
            OnCommandModeChanged?.Invoke(IsCommandMode);
        }

        public void BlockJumpUntilRelease()
        {
            _jumpBlocked = true;
        }

        public void SetDialogueMode(bool active)
        {
            _isDialogueMode = active;
        }

        #endregion
    }
}
