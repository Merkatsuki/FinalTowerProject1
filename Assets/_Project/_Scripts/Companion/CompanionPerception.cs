using System.Collections.Generic;
using UnityEngine;

public class CompanionPerception : MonoBehaviour
{
    [Header("Detection Settings")]
    public float perceptionRadius = 5f;
    public LayerMask detectionMask;
    public float checkInterval = 0.2f;

    private SortedList<float, IRobotPerceivable> currentTargets = new SortedList<float, IRobotPerceivable>();
    private HashSet<IRobotPerceivable> handledTargets = new HashSet<IRobotPerceivable>();
    private float checkTimer;
    private CompanionController controller;

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
        currentTargets.Clear();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, perceptionRadius, detectionMask);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IRobotPerceivable>(out var target) && target.IsAvailable() && !HasBeenHandled(target))
            {
                float score = target.GetPriority(); // Add distance penalty here if desired
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
        if (target != null)
            handledTargets.Add(target);
    }

    public bool HasBeenHandled(IRobotPerceivable target)
    {
        return handledTargets.Contains(target);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, perceptionRadius);
    }
#endif
}