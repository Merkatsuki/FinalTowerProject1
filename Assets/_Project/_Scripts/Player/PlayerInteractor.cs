using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Momentum;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private CompanionController companion;
    [SerializeField] private CameraController cameraController;

    [Header("Facing Settings")]
    [SerializeField] private Transform visionConeObject;
    [SerializeField] private float facingThreshold = 0.5f;

    [Header("Interaction Settings")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference rightClickAction;
    [SerializeField] private InputActionReference leftClickAction;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private InteractionPromptUI promptUI;
    [SerializeField] private float clickMarkerLifetime = 1.5f;
    [SerializeField] private float interactRadius = 4f;

    [Header("Command Mode Settings")]
    [SerializeField] private GameObject clickMarkerPrefab;
    [SerializeField] private CanvasGroup commandOverlay;
    [SerializeField] private float commandModeRadiusBoost = 2f;
    [SerializeField] private float commandModeFacingBonus = 0.2f;

    [Header("Gizmo Settings")]
    [SerializeField] private float markerRadius = 0.2f;

    private List<IWorldInteractable> nearbyInteractables = new();
    private IWorldInteractable currentTarget;
    private Player player;
    private CircleCollider2D triggerCollider;
    private bool wasCommandModeActive = false;

    private void OnEnable()
    {
        interactAction.action.Enable();
        interactAction.action.performed += OnInteractPressed;
        rightClickAction.action.performed += HandleRightClick;
        leftClickAction.action.performed += HandleLeftClick;
    }

    private void OnDisable()
    {
        interactAction.action.performed -= OnInteractPressed;
        rightClickAction.action.performed -= HandleRightClick;
        leftClickAction.action.performed -= HandleLeftClick;
        InputManager.instance.OnCommandModeChanged -= HandleCommandModeChanged;
        interactAction.action.Disable();
    }

    private void OnValidate()
    {
        if (InputManager.instance != null)
            SyncColliderRadius();
    }

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        SyncColliderRadius();

        InputManager.instance.OnCommandModeChanged += HandleCommandModeChanged;
    }

    private void SyncColliderRadius()
    {
        if (triggerCollider == null)
            triggerCollider = GetComponent<CircleCollider2D>();

        if (triggerCollider != null)
        {
            float maxScale = Mathf.Max(transform.localScale.x, transform.localScale.y);
            float radius = interactRadius + (InputManager.instance.IsCommandMode ? commandModeRadiusBoost : 0f);
            triggerCollider.radius = radius / maxScale;
        }
    }

    void Update()
    {
        UpdateFacingDirection();
        UpdateFocus();
        UpdateCollider(); 

    }


    private void UpdateCollider()
    {
        bool isCommanding = InputManager.instance != null && InputManager.instance.IsCommandMode;

        if (isCommanding != wasCommandModeActive)
        {
            wasCommandModeActive = isCommanding;
            SyncColliderRadius();
        }
    }

    private void HandleRightClick(InputAction.CallbackContext context)
    {
        if (!InputManager.instance.IsCommandMode) return;
        TryIssueCommandToCompanion();
    }

    private void HandleLeftClick(InputAction.CallbackContext context)
    {
        if (!InputManager.instance.IsCommandMode) return;
        OnInteractPressed(context);
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
        if (!Application.isPlaying) return;

        Camera renderCamera = Camera.main;
        if (renderCamera == null) return;

        Vector3 mouseScreenPos = InputManager.instance.MousePosition;
        Vector3 mouseWorldPos = renderCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        Vector2 direction = (mouseWorldPos - visionConeObject.position).normalized;
        visionConeObject.right = direction;

        Debug.DrawLine(visionConeObject.position, mouseWorldPos, Color.green);
    }

    private void UpdateFocus()
    {
        IWorldInteractable best = GetBestInteractable();

        bool shouldUpdate =
            (best == null && currentTarget != null) ||
            (best != null && best != currentTarget);   

        if (!shouldUpdate) return;

        if (currentTarget != null)
            currentTarget.SetHighlight(false);

        currentTarget = best;

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

    private float GetCurrentInteractRadius()
    {
        return interactRadius + (InputManager.instance.IsCommandMode ? commandModeRadiusBoost : 0f);
    }

    private void HandleCommandModeChanged(bool isCommandMode)
    {
        // Update camera and visuals
        cameraController?.SetCameraMode(isCommandMode);

        UpdateCommandOverlay(isCommandMode);

        // Optional: adjust collider
        SyncColliderRadius();
    }

    private void UpdateCommandOverlay(bool isCommandMode)
    {
        commandOverlay.alpha = isCommandMode ? 1f : 0f;
        commandOverlay.interactable = false;
        commandOverlay.blocksRaycasts = false;
    }

    private void OnInteractPressed(InputAction.CallbackContext context)
    {
        if (currentTarget != null && currentTarget.CanBeInteractedWith(player))
        {
            float distToTarget = Vector2.Distance(transform.position, currentTarget.GetTransform().position);
            if (distToTarget <= GetCurrentInteractRadius())
            {
                currentTarget.OnInteract(player);
            }
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

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (visionConeObject == null) return;

        Vector3 origin = visionConeObject.position;
        Vector3 forward = visionConeObject.right;

        float displayRadius = interactRadius + (Application.isPlaying && InputManager.instance != null && InputManager.instance.IsCommandMode ? commandModeRadiusBoost : 0f);
        float angle = Mathf.Acos(facingThreshold) * Mathf.Rad2Deg;
        float coneLength = 4f + (Application.isPlaying && InputManager.instance != null && InputManager.instance.IsCommandMode ? commandModeRadiusBoost : 0f);

        Gizmos.color = new Color(1f, 1f, 1f, 0.1f);
        Gizmos.DrawWireSphere(origin, displayRadius);

        Handles.color = new Color(0f, 1f, 1f, 0.2f);
        Vector3 left = Quaternion.Euler(0, 0, -angle) * forward;
        Handles.DrawSolidArc(origin, Vector3.forward, left, angle * 2, coneLength);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, origin + forward * coneLength);

        foreach (var interactable in nearbyInteractables)
        {
            if (interactable == null) continue;

            Vector3 targetPos = interactable.GetTransform().position;
            Vector3 toTarget = (targetPos - origin).normalized;
            float dot = Vector2.Dot(forward, toTarget);

            Gizmos.color = (dot > facingThreshold) ? Color.yellow : Color.red;
            if (interactable == currentTarget) Gizmos.color = Color.green;

            Gizmos.DrawLine(origin, targetPos);
            Handles.Label(targetPos + Vector3.up * 0.2f, $"Dot: {dot:F2}");
        }
#endif
    }


}