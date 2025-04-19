using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerInteractor : MonoBehaviour
{
    [Header("Facing Settings")]
    [SerializeField] private Transform visionConeObject;
    [SerializeField] private float facingThreshold = 0.5f;

    [Header("Interaction Settings")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private InteractionPromptUI promptUI;

    private List<InteractableBase> nearbyInteractables = new();
    private InteractableBase currentTarget;

    private void OnEnable()
    {
        interactAction.action.Enable();
        interactAction.action.performed += OnInteractPressed;
    }

    private void OnDisable()
    {
        interactAction.action.performed -= OnInteractPressed;
        interactAction.action.Disable();
    }

    void Update()
    {
        UpdateFacingDirection();
        UpdateFocus();
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
        InteractableBase best = GetBestInteractable();

        if (best != currentTarget)
        {
            currentTarget?.OnFocusExit();
            currentTarget = best;
            if (currentTarget != null)
                promptUI?.Show(currentTarget.promptMessage);
            else
                promptUI?.Hide();
            currentTarget?.OnFocusEnter();
        }
    }

    private void OnInteractPressed(InputAction.CallbackContext context)
    {
        if (currentTarget != null)
        {
            currentTarget.OnInteract();
        }
    }

    private InteractableBase GetBestInteractable()
    {
        InteractableBase best = null;
        float bestDot = -1f;

        foreach (var interactable in nearbyInteractables)
        {
            if (interactable == null || !interactable.CanInteract) continue;

            Vector2 toTarget = (interactable.transform.position - visionConeObject.position).normalized;
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
        if (other.TryGetComponent(out InteractableBase interactable) && !nearbyInteractables.Contains(interactable))
        {
            nearbyInteractables.Add(interactable);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out InteractableBase interactable))
        {
            nearbyInteractables.Remove(interactable);
            if (currentTarget == interactable)
            {
                currentTarget?.OnFocusExit();
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

        // Draw the main facing direction line
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, origin + forward * 4f);

        // Draw the facing cone arc
        Handles.color = new Color(0f, 1f, 1f, 0.2f);
        float angle = Mathf.Acos(facingThreshold) * Mathf.Rad2Deg;
        Vector3 left = Quaternion.Euler(0, 0, -angle) * forward;
        Handles.DrawSolidArc(origin, Vector3.forward, left, angle * 2, 4f);

        // Draw lines to each nearby interactable
        foreach (var interactable in nearbyInteractables)
        {
            if (interactable == null) continue;

            Vector3 targetPos = interactable.transform.position;
            Vector3 toTarget = (targetPos - origin).normalized;
            float dot = Vector2.Dot(forward, toTarget);

            // Use color coding
            if (dot > facingThreshold)
            {
                // Valid target
                Gizmos.color = (interactable == currentTarget) ? Color.green : Color.yellow;
            }
            else
            {
                Gizmos.color = Color.red;
            }

            Gizmos.DrawLine(origin, targetPos);
        }
    }
#endif
}
