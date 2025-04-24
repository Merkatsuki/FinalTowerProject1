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
    [SerializeField] private GameObject clickMarkerPrefab;
    [SerializeField] private float clickMarkerLifetime = 1.5f;
    [SerializeField] private float interactRadius = 2f;

    [Header("Gizmo Settings")]
    [SerializeField] private float markerRadius = 0.2f;

    private List<IWorldInteractable> nearbyInteractables = new();
    private IWorldInteractable currentTarget;
    private Player player;
    private CircleCollider2D triggerCollider;

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

    private void OnValidate()
    {
        SyncColliderRadius();
    }

    private void Awake()
    {
        player = GetComponent<Player>();

        SyncColliderRadius();
    }

    private void SyncColliderRadius()
    {
        if (triggerCollider == null)
            triggerCollider = GetComponent<CircleCollider2D>();

        if (triggerCollider != null)
        {
            // Compensate for the largest axis of local scale
            float maxScale = Mathf.Max(transform.localScale.x, transform.localScale.y);
            triggerCollider.radius = interactRadius / maxScale;
        }
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
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(InputManager.instance.MousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if (hit.collider != null)
        {
            var target = hit.collider.GetComponent<IWorldInteractable>();
            if (target != null && target.CanBeInteractedWith(companion))
            {
                companion.IssuePlayerCommand(target);
                Debug.Log($"[Command] Companion ordered to interact with: {target.GetDisplayName()}");
                return;
            }
        }

        // If no valid interactable, still move to the clicked world point
        companion.CommandMoveToPoint(mouseWorldPos);
        SpawnClickMarker(mouseWorldPos);
        Debug.Log($"[Command] Companion moving to point: {mouseWorldPos}");
    }

    void SpawnClickMarker(Vector2 worldPos)
    {
        if (clickMarkerPrefab == null) return;
        var marker = Instantiate(clickMarkerPrefab, worldPos, Quaternion.identity);
        Destroy(marker, clickMarkerLifetime);
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

        bool shouldUpdate =
            (best == null && currentTarget != null) || // We had a target, now we don’t
            (best != null && best != currentTarget);   // Target changed

        if (!shouldUpdate) return;

        // Disable old highlight
        if (currentTarget != null)
            currentTarget.SetHighlight(false);

        currentTarget = best;

        // Enable new highlight
        if (currentTarget != null)
        {
            currentTarget.SetHighlight(true);
            promptUI?.Show(currentTarget.GetDisplayName());
        }
        else
        {
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

            //if (IsWithinFacingCone(toTarget) && dot > bestDot)
            if (dot > facingThreshold && dot > bestDot)
            {
                best = interactable;
                bestDot = dot;
            }
        }

        return best;
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
            if (currentTarget != null && !nearbyInteractables.Contains(currentTarget))
            {
                currentTarget.SetHighlight(false);
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

        // Draw cone direction line
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, origin + forward * 4f);

        // Draw interaction radius
        Gizmos.color = new Color(1f, 1f, 1f, 0.1f);
        Gizmos.DrawWireSphere(origin, interactRadius);

        // Draw cone arc
        UnityEditor.Handles.color = new Color(0f, 1f, 1f, 0.2f);
        float angle = Mathf.Acos(facingThreshold) * Mathf.Rad2Deg;
        Vector3 left = Quaternion.Euler(0, 0, -angle) * forward;
        UnityEditor.Handles.DrawSolidArc(origin, Vector3.forward, left, angle * 2, 4f);

        // Per-interactable debug
        foreach (var interactable in nearbyInteractables)
        {
            if (interactable == null) continue;

            Vector3 targetPos = interactable.GetTransform().position;
            Vector3 toTarget = (targetPos - origin).normalized;
            float dot = Vector2.Dot(forward, toTarget);

            Gizmos.color = (dot > facingThreshold) ? Color.yellow : Color.red;
            if (interactable == currentTarget) Gizmos.color = Color.green;

            Gizmos.DrawLine(origin, targetPos);

            // Optional debug label
            UnityEditor.Handles.Label(targetPos + Vector3.up * 0.2f, $"Dot: {dot:F2}");
        }
    }
#endif

}