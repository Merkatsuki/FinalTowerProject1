using System.Collections.Generic;
using UnityEngine;

public class CompanionPerception : MonoBehaviour
{
    [Header("Detection Settings")]
    public float perceptionRadius = 5f;
    public LayerMask detectionMask;
    public float checkInterval = 0.2f;

    private List<IPerceivable> currentTargets = new List<IPerceivable>();
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
            IPerceivable target = hit.GetComponent<IPerceivable>();
            if (target != null && target.IsAvailable())
            {
                currentTargets.Add(target);
            }
        }

        if (currentTargets.Count > 0)
        {
            IPerceivable highest = GetHighestPriorityTarget();
            controller.SetTargetToInvestigate(highest);
        }
        else
        {
            controller.ClearInvestigationTarget();
        }
    }

    private IPerceivable GetHighestPriorityTarget()
    {
        IPerceivable best = null;
        float highest = float.MinValue;

        foreach (var target in currentTargets)
        {
            float p = target.GetPriority();
            if (p > highest)
            {
                highest = p;
                best = target;
            }
        }

        return best;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, perceptionRadius);
    }
#endif
}
