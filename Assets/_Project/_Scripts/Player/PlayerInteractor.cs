using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Momentum;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private CompanionController companion;

    [Header("Facing Settings")]
    [SerializeField] private Transform visionConeObject;
    [SerializeField] private float facingThreshold = 0.5f;

    [Header("Interaction Settings")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference rightClickAction;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private InteractionPromptUI promptUI;
    [SerializeField] private GameObject commandOverlay;


    private List<IWorldInteractable> nearbyInteractables = new();
    private IWorldInteractable currentTarget;
    private Player player;

    private void OnEnable()
    {
        interactAction.action.Enable();
        interactAction.action.performed += OnInteractPressed;
        rightClickAction.action.performed += HandleRightClick;
    }

    private void OnDisable()
    {
        interactAction.action.performed -= OnInteractPressed;
        rightClickAction.action.performed -= HandleRightClick;
        interactAction.action.Disable();
    }

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    void Update()
    {
        UpdateFacingDirection();
        UpdateFocus();
        
        commandOverlay?.SetActive(InputManager.instance.IsCommandMode);     

    }

    private void HandleRightClick(InputAction.CallbackContext context)
    {
        if (!InputManager.instance.IsCommandMode) return;
        TryIssueCommandToCompanion();
    }

    private void TryIssueCommandToCompanion()
    {
        Debug.Log($"[PlayerInteractor] Attempting to issue command to companion.");
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var target = hit.collider.GetComponent<IWorldInteractable>();
            if (target != null && target.CanBeInteractedWith(companion))
            {
                companion.IssuePlayerCommand(target);
                Debug.Log($"[PlayerInteractor] Commanded companion to interact with: {target.GetDisplayName()}");
            }
        }
    }

    private void UpdateFacingDirection()
    {
        if (mainCamera == null) return;

        Vector3 mouseWorldPos = Mouse.current.position.ReadValue();
        mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseWorldPos);
        mouseWorldPos.z = 0f;

        Vector2 direction = (mouseWorldPos - visionConeObject.position).normalized;
        if (direction.sqrMagnitude > 0.001f)
        {
            visionConeObject.right = direction;
        }
    }

    private void UpdateFocus()
    {
        IWorldInteractable best = GetBestInteractable();

        if (best != currentTarget)
        {
            currentTarget = best;
            if (currentTarget != null)
                promptUI?.Show(currentTarget.GetDisplayName());
            else
                promptUI?.Hide();
        }
    }

    private void OnInteractPressed(InputAction.CallbackContext context)
    {
        if (currentTarget != null && currentTarget.CanBeInteractedWith(player))
        {
            currentTarget.OnInteract(player);
        }
    }

    private IWorldInteractable GetBestInteractable()
    {
        IWorldInteractable best = null;
        float bestDot = -1f;

        foreach (var interactable in nearbyInteractables)
        {
            if (interactable == null) continue;

            Vector2 toTarget = (interactable.GetTransform().position - visionConeObject.position).normalized;
            float dot = Vector2.Dot(visionConeObject.right, toTarget);

            if (IsWithinFacingCone(toTarget) && dot > bestDot)
            {
                best = interactable;
                bestDot = dot;
            }
        }

        return best;
    }

    private bool IsWithinFacingCone(Vector2 toTarget)
    {
        float dot = Vector2.Dot(visionConeObject.right, toTarget.normalized);
        return dot > facingThreshold;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IWorldInteractable interactable) && !nearbyInteractables.Contains(interactable))
        {
            nearbyInteractables.Add(interactable);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out IWorldInteractable interactable))
        {
            nearbyInteractables.Remove(interactable);
            if (currentTarget == interactable)
            {
                currentTarget = null;
                promptUI?.Hide();
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (visionConeObject == null) return;

        Vector3 origin = visionConeObject.position;
        Vector3 forward = visionConeObject.right;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, origin + forward * 4f);

        Handles.color = new Color(0f, 1f, 1f, 0.2f);
        float angle = Mathf.Acos(facingThreshold) * Mathf.Rad2Deg;
        Vector3 left = Quaternion.Euler(0, 0, -angle) * forward;
        Handles.DrawSolidArc(origin, Vector3.forward, left, angle * 2, 4f);

        foreach (var interactable in nearbyInteractables)
        {
            if (interactable == null) continue;

            Vector3 targetPos = interactable.GetTransform().position;
            Vector3 toTarget = (targetPos - origin).normalized;
            float dot = Vector2.Dot(forward, toTarget);

            Gizmos.color = (dot > facingThreshold) ? Color.yellow : Color.red;
            if (interactable == currentTarget) Gizmos.color = Color.green;

            Gizmos.DrawLine(origin, targetPos);
        }
    }
#endif
}