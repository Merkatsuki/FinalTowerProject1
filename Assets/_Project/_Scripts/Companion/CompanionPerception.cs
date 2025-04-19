using System.Collections.Generic;
using UnityEngine;

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

    private readonly SortedList<float, IRobotPerceivable> currentTargets = new();
    private readonly HashSet<IRobotPerceivable> handledTargets = new();

    private CompanionController controller;
    private float checkTimer;


    #region Initialization
    private void Awake()
    {
        controller = GetComponent<CompanionController>();
    }


    #endregion

    #region Update Loop
    private void Update()
    {
        checkTimer -= Time.deltaTime;
        if (checkTimer <= 0f)
        {
            checkTimer = checkInterval;
            ScanForTargets();
        }
    }


    #endregion

    #region Target Scanning
    private void ScanForTargets()
    {
        if (controller.IsInteractionLocked) return;

        currentTargets.Clear();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, perceptionRadius, detectionMask);
        foreach (var hit in hits)
        {
            if (
                hit.TryGetComponent<IRobotPerceivable>(out var target)
                && !HasBeenHandled(target)
            )
            {
                if (target is CompanionClueInteractable clue)
                {
                    bool hasValidInteraction = false;
                    foreach (var interaction in clue.GetRobotInteractions())
                    {
                        if (interaction.CanExecute(controller, clue) && !clue.HasUsedInteraction(interaction))
                        {
                            hasValidInteraction = true;
                            break;
                            // ⬆️ End ScanForTargets
                        }
                    }

                    if (!hasValidInteraction)
                        continue;
                }

                float score = target.GetPriority();
                while (currentTargets.ContainsKey(score)) score += 0.001f;
                currentTargets.Add(score, target);
            }
        }
    }

    public IRobotPerceivable GetCurrentTarget()
    {
        return currentTargets.Count > 0 ? currentTargets.Values[^1] : null;
    }

    public void MarkAsHandled(IRobotPerceivable target)
    {
        if (target is CompanionClueInteractable clue && clue.TryGetComponent<EnergyDockingZone>(out _))
        {
            return;
        }

        if (!handledTargets.Contains(target))
        {
            handledTargets.Add(target);
        }
    }

    public bool HasBeenHandled(IRobotPerceivable target) => handledTargets.Contains(target);

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        Gizmos.color = perceptionRadiusColor;
        Gizmos.DrawWireSphere(transform.position, perceptionRadius);

        if (!Application.isPlaying) return;

        // All perceived targets
        foreach (var kvp in currentTargets)
        {
            var target = kvp.Value;
            var pos = target.GetTransform().position;

            // Differentiate handled targets
            Gizmos.color = HasBeenHandled(target) ? handledTargetColor : allTargetLineColor;
            Gizmos.DrawLine(transform.position, pos);
            Gizmos.DrawSphere(pos, 0.1f);
        }

        // Highlight current target
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

#endregion
