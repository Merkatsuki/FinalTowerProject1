﻿using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Momentum;
using System;
using DG.Tweening;
using static UnityEditor.Experimental.GraphView.GraphView;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerInteractor : MonoBehaviour
{
    public bool PuzzleModeOn;
    [SerializeField] private CompanionController companion;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Player player;

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
    [SerializeField] private LayerMask interactableLayerMask;

    [Header("Command Mode Settings")]
    [SerializeField] private GameObject clickMarkerPrefab;
    [SerializeField] private CanvasGroup commandOverlay;
    [SerializeField] private CanvasGroup commandOverlayPanel;
    [SerializeField] private float commandModeRadiusBoost = 2f;
    [SerializeField] private float commandModeFacingBonus = 0.2f;

    [Header("Gizmo Settings")]
    [SerializeField] private float markerRadius = 0.2f;

    private List<IWorldInteractable> nearbyInteractables = new();
    private IWorldInteractable currentTarget;
    private IWorldInteractable lastHoveredTarget;
    public IWorldInteractable commandHoverTarget;
    private CircleCollider2D triggerCollider;
    private bool wasCommandModeActive = false;

    public CompanionController Companion => companion;
    public LayerMask InteractableMask => interactableLayerMask;

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

    private void Start()
    {
        SyncColliderRadius();

        InputManager.instance.OnCommandModeChanged += HandleCommandModeChanged;
        ZoneManager.Instance.OnAngerZoneExited += HandleAngerZoneExit;

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

    public void EnterPuzzle() => PuzzleModeOn = true;
    public void ExitPuzzle() => PuzzleModeOn = false;

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
        if(PuzzleModeOn) return; // Disable left click in puzzle mode
        if (IsPointerOverBlockingUI()) return;

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(InputManager.instance.MousePosition);
        companion.CommandMoveToPoint(mouseWorldPos);
        SpawnClickMarker(mouseWorldPos);
    }

    private bool IsPointerOverBlockingUI()
    {
        var eventSystem = UnityEngine.EventSystems.EventSystem.current;
        if (eventSystem == null) return false;

        var pointerData = new UnityEngine.EventSystems.PointerEventData(eventSystem)
        {
            position = InputManager.instance.MousePosition
        };

        var results = new List<UnityEngine.EventSystems.RaycastResult>();
        UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            // Add tag or layer filter here if needed
            if (result.gameObject.CompareTag("BlockInput"))
                return true;
        }

        return false;
    }

    private void TryIssueCommandToCompanion()
    {
        Vector3 mouseScreenPos = InputManager.instance.MousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);

        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 100f, interactableLayerMask);

        if (hit.collider != null)
        {
            var target = hit.collider.GetComponent<IWorldInteractable>();
            if (companion.Perception.CanInteractWith(target))
            {
                companion.IssuePlayerCommand(target);
                Debug.Log($"[Command] Companion ordered to interact with: {target.GetDisplayName()}");
                return;
            }
        }
    }

    void SpawnClickMarker(Vector2 worldPos)
    {
        if (clickMarkerPrefab == null) return;
        var marker = Instantiate(clickMarkerPrefab, worldPos, Quaternion.identity);
        Destroy(marker, clickMarkerLifetime);
    }

    private void UpdateFacingDirection()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        Vector3 mouseScreenPos = InputManager.instance.MousePosition;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        Vector3 anchorPos = InputManager.instance.IsCommandMode && companion != null
            ? companion.transform.position
            : visionConeObject.position;

        Vector2 direction = (mouseWorldPos - anchorPos).normalized;
        visionConeObject.right = direction;

        Debug.DrawLine(anchorPos, mouseWorldPos, Color.green);
    }

    private void UpdateFocus()
    {
        IWorldInteractable hoverTarget = GetHoveredInteractable();

        if (InputManager.instance.IsCommandMode)
        {
            if (lastHoveredTarget != null && lastHoveredTarget != hoverTarget)
            {
                lastHoveredTarget.SetHighlight(false);
            }

            if (hoverTarget != null)
            {
                hoverTarget.SetHighlight(true);
            }

            lastHoveredTarget = hoverTarget;
            return;
        }

        // ---- Normal gameplay targeting ----

        IWorldInteractable newTarget = GetBestInteractable();
        var hovered = GetHoveredInteractable();
        if (hovered != null && nearbyInteractables.Contains(hovered))
        {
            newTarget = hovered;
        }

        if (newTarget == currentTarget) return;

        if (currentTarget != null)
            currentTarget.SetHighlight(false);

        currentTarget = newTarget;

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

    private IWorldInteractable GetHoveredInteractable()
    {
        Vector3 mouseScreenPos = InputManager.instance.MousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.cyan);

        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 100f, interactableLayerMask);

        if (hit.collider != null)
        {
            return hit.collider.GetComponentInParent<IWorldInteractable>();
        }

        return null;
    }

    public void UpdateCommandHover()
    {
        var mouseWorld = Camera.main.ScreenToWorldPoint(InputManager.instance.MousePosition);
        var hit = Physics2D.OverlapPoint(mouseWorld, interactableLayerMask);

        IWorldInteractable newHover = null;

        if (hit != null && hit.TryGetComponent(out IWorldInteractable candidate))
        {
            if (companion.Perception.CanInteractWith(candidate))
            {
                newHover = candidate;
            }
        }

        if (commandHoverTarget != newHover)
        {
            if (commandHoverTarget != null)
                commandHoverTarget.SetHighlight(false);

            if (newHover != null)
                newHover.SetHighlight(true);

            commandHoverTarget = newHover;
        }
    }

    private float GetCurrentInteractRadius()
    {
        return interactRadius + (InputManager.instance.IsCommandMode ? commandModeRadiusBoost : 0f);
    }

    private void HandleCommandModeChanged(bool isCommandMode)
    {
        cameraController?.SetCameraMode(isCommandMode);
        UpdateCommandOverlay(isCommandMode);
        SyncColliderRadius();

        if (isCommandMode)
        {
            CompanionCommandManager.Instance?.EnterCommandMode();
            player.StateMachine.ChangeState(player.CommandState);
        }
        else
        {
            LoreViewerUI.Instance?.Close();
            CompanionCommandManager.Instance?.ExitCommandMode();
            player.StateMachine.ChangeState(player.IdleState);
        }
    }

    private void UpdateCommandOverlay(bool isCommandMode)
    {
        commandOverlayPanel.DOFade(isCommandMode ? 1f : 0f, 0.3f).SetEase(Ease.OutQuad).SetUpdate(true);


        commandOverlay.DOFade(isCommandMode ? 1f : 0f, 0.3f)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);

        commandOverlay.interactable = isCommandMode;
        commandOverlay.blocksRaycasts = isCommandMode;
    }

    public void EnterAngerPuzzleMode(Transform cameraTarget)
    {
        PuzzleModeOn = true;
        cameraController?.FollowActiveCameraTarget(cameraTarget);
        GameStateManager.Instance.SetState(GameState.PuzzleInteraction);
    }

    private void HandleAngerZoneExit(ZoneTag newZone)
    {
        ExitPuzzle();
        GameStateManager.Instance.SetState(GameState.Gameplay);

        // Teleport companion to follow target
        Transform followTarget = companion.defaultFollowTarget;
        if (followTarget != null)
        {
            companion.Agent.Warp(followTarget.position);
        }
    }

    private void OnInteractPressed(InputAction.CallbackContext context)
    {
        if (InputManager.instance.IsCommandMode)
            return; // no interaction in command mode

        currentTarget?.OnInteract(player);
    }

    private IWorldInteractable GetBestInteractable()
    {
        IWorldInteractable best = null;
        float bestDot = -1f;

        Vector3 origin = InputManager.instance.IsCommandMode && companion != null
            ? companion.transform.position
            : visionConeObject.position;

        Vector3 forward = visionConeObject.right;

        foreach (var interactable in nearbyInteractables)
        {
            if (interactable == null) continue;

            Vector2 toTarget = (interactable.GetTransform().position - origin).normalized;
            float dot = Vector2.Dot(forward, toTarget);

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

        Vector3 origin = InputManager.instance != null && InputManager.instance.IsCommandMode && companion != null
            ? companion.transform.position
            : visionConeObject.position;

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
