using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float facingThreshold = 0.5f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("References")]
    [SerializeField] private Transform facingDirection; // Reference to player facing transform
    [SerializeField] private InteractionPromptUI promptUI;

    private List<InteractableBase> nearbyInteractables = new();
    private InteractableBase currentTarget;

    void Update()
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

        if (Input.GetKeyDown(interactKey) && currentTarget != null)
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
            if (interactable == null) continue;

            Vector2 toTarget = (interactable.transform.position - transform.position).normalized;
            float dot = Vector2.Dot(facingDirection.right, toTarget);

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
}
