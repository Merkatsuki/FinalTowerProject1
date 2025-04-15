using System.Collections.Generic;
using UnityEngine;

public class CompanionPerception : MonoBehaviour
{
    [Header("Detection Settings")]
    public float perceptionRadius = 5f;
    public LayerMask detectionMask;
    public float checkInterval = 0.2f;

    private SortedList<float, IPerceivable> currentTargets = new SortedList<float, IPerceivable>();
    private HashSet<IPerceivable> handledTargets = new HashSet<IPerceivable>();
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
            if (hit.TryGetComponent<IPerceivable>(out var target) && target.IsAvailable() && !HasBeenHandled(target))
            {
                float score = target.GetPriority(); // Add distance penalty here if desired
                while (currentTargets.ContainsKey(score)) score += 0.001f;
                currentTargets.Add(score, target);
            }
        }
    }

    public IPerceivable GetCurrentTarget()
    {
        return currentTargets.Count > 0 ? currentTargets.Values[^1] : null;
    }

    public void MarkAsHandled(IPerceivable target)
    {
        if (target != null)
            handledTargets.Add(target);
    }

    public bool HasBeenHandled(IPerceivable target)
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