using System.Collections.Generic;
using UnityEngine;

public class CompanionPerception : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float perceptionRadius = 5f;
    [SerializeField] private LayerMask detectionMask;
    [SerializeField] private float checkInterval = 0.2f;


    private readonly SortedList<float, IRobotPerceivable> currentTargets = new();
    private readonly HashSet<IRobotPerceivable> handledTargets = new();

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
                hit.TryGetComponent<IRobotPerceivable>(out var target)
                && !HasBeenHandled(target)
                && target.IsAvailable()
                            )
            {
                // Skip clue-wrapped docking zones if already charged
                if (controller.GetEnergyType() != EnergyType.None)
                {
                    if (target is CompanionClueInteractable clue && clue.TryGetComponent<EnergyDockingZone>(out _))
                    {
                        //Debug.Log($"Skipping energy dock {clue.name} because companion is already charged.");
                        continue;
                    }
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
            // Don't mark docking zones as handled so they can be reused
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
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, perceptionRadius);
    }
#endif
}