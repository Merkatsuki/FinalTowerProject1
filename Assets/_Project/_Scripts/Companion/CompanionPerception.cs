using System.Collections.Generic;
using UnityEngine;

public struct TargetData
{
    public Vector2 Position;
    public IWorldInteractable Interactable;
    public IHoverProfileProvider HoverProfile;

    public TargetData(Vector2 position, IWorldInteractable interactable = null, IHoverProfileProvider hoverProfile = null)
    {
        Position = position;
        Interactable = interactable;
        HoverProfile = hoverProfile;
    }

    public bool HasInteractable => Interactable != null;
}

public class CompanionPerception : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float perceptionRadius = 5f;
    [SerializeField] private LayerMask detectionMask;
    [SerializeField] private float checkInterval = 0.2f;

    [Header("Debug Settings")]
    [SerializeField] private bool drawGizmos = true;
    [SerializeField] private Color perceptionRadiusColor = Color.magenta;
    [SerializeField] private Color allTargetLineColor = Color.yellow;
    [SerializeField] private Color currentTargetLineColor = Color.green;
    [SerializeField] private Color handledTargetColor = Color.gray;

    private readonly SortedList<float, IWorldInteractable> currentTargets = new();
    private readonly HashSet<IWorldInteractable> handledTargets = new();

    private CompanionController controller;
    private float checkTimer;

    private void Awake()
    {
        controller = GetComponent<CompanionController>();
    }

    private void Update()
    {
        checkTimer -= Time.deltaTime;
        if (checkTimer <= 0f)
        {
            checkTimer = checkInterval;
            ScanForTargets();
        }
    }

    private void ScanForTargets()
    {
        if (controller.IsInteractionLocked) return;

        currentTargets.Clear();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, perceptionRadius, detectionMask);
        foreach (var hit in hits)
        {
            if (
                hit.TryGetComponent<IWorldInteractable>(out var target)
                && !handledTargets.Contains(target)
                && target.CanBeInteractedWith(controller)
            )
            {
                float score = Vector2.Distance(transform.position, target.GetTransform().position);
                while (currentTargets.ContainsKey(score)) score += 0.001f;
                currentTargets.Add(score, target);
            }
        }
    }

    public IWorldInteractable GetCurrentTarget()
    {
        return currentTargets.Count > 0 ? currentTargets.Values[^1] : null;
    }

    public void MarkAsHandled(IWorldInteractable target)
    {
        if (!handledTargets.Contains(target))
        {
            handledTargets.Add(target);
        }
    }

    public bool HasBeenHandled(IWorldInteractable target) => handledTargets.Contains(target);

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        Gizmos.color = perceptionRadiusColor;
        Gizmos.DrawWireSphere(transform.position, perceptionRadius);

        if (!Application.isPlaying) return;

        foreach (var kvp in currentTargets)
        {
            var target = kvp.Value;
            var pos = target.GetTransform().position;

            Gizmos.color = HasBeenHandled(target) ? handledTargetColor : allTargetLineColor;
            Gizmos.DrawLine(transform.position, pos);
            Gizmos.DrawSphere(pos, 0.1f);
        }

        var current = GetCurrentTarget();
        if (current != null)
        {
            Gizmos.color = currentTargetLineColor;
            Gizmos.DrawLine(transform.position, current.GetTransform().position);
            Gizmos.DrawSphere(current.GetTransform().position, 0.2f);
        }
    }
#endif
}