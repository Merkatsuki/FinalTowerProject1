using System.Collections.Generic;
using UnityEngine;

public class CompanionPerception : MonoBehaviour
{
    [Header("Detection Settings")]
    public float checkInterval = 0.5f;
    public float detectionRadius = 4f;
    public LayerMask interactableMask;

    private CompanionController companion;
    private float checkTimer;
    private readonly List<IWorldInteractable> nearbyInteractables = new();
    private readonly HashSet<IWorldInteractable> handledInteractables = new();

    private void Update()
    {
        checkTimer += Time.deltaTime;

        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;
            RefreshNearbyInteractables();
        }
    }

    private void Awake()
    {
        companion = GetComponent<CompanionController>();
    }

    private void RefreshNearbyInteractables()
    {
        nearbyInteractables.Clear();
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, interactableMask);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IWorldInteractable>(out var interactable))
            {
                nearbyInteractables.Add(interactable);
            }
        }
    }

    public IReadOnlyList<IWorldInteractable> GetNearbyInteractables() => nearbyInteractables;

    public void MarkAsHandled(IWorldInteractable target)
    {
        if (target != null && !handledInteractables.Contains(target))
        {
            handledInteractables.Add(target);
        }
    }

    public bool HasBeenHandled(IWorldInteractable target)
    {
        return target != null && handledInteractables.Contains(target);
    }

    public bool CanInteractWith(IWorldInteractable target)
    {
        if (target == null) return false;
        return target.CanBeInteractedWith(companion);
    }
}
