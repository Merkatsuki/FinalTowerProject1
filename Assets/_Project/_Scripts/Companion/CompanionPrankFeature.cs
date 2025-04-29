using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CompanionPrankFeature : MonoBehaviour
{
    [Header("Prank Settings")]
    [SerializeField] private bool pranksEnabled = true;
    [SerializeField] private float prankIntervalMin = 10f;
    [SerializeField] private float prankIntervalMax = 30f;
    [SerializeField] private float prankRange = 5f;

    [Header("Prank Actions")]
    [SerializeField] private List<GameObject> prankTargets = new(); // Objects the companion can interact with

    private Transform companionTransform;
    private Coroutine prankCoroutine;

    private void Start()
    {
        companionTransform = FindFirstObjectByType<CompanionController>()?.transform;

        if (companionTransform == null)
        {
            Debug.LogWarning("[CompanionPrankFeature] Companion not found in scene!");
            return;
        }

        if (pranksEnabled)
        {
            prankCoroutine = StartCoroutine(PrankRoutine());
        }
    }

    private IEnumerator PrankRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(prankIntervalMin, prankIntervalMax);
            yield return new WaitForSeconds(waitTime);

            AttemptPrank();
        }
    }

    private void AttemptPrank()
    {
        if (companionTransform == null) return;

        GameObject closestTarget = FindClosestPrankTarget();

        if (closestTarget != null)
        {
            Debug.Log("[CompanionPrankFeature] Companion playing prank!");

            // Simple prank example: "push" object or play animation
            if (closestTarget.TryGetComponent<Rigidbody2D>(out var rb))
            {
                Vector2 prankDirection = (Vector2)(closestTarget.transform.position - companionTransform.position).normalized;
                rb.AddForce(prankDirection * 2f, ForceMode2D.Impulse);
            }
            else
            {
                // If no rigidbody, maybe just nudge position a little for fun
                closestTarget.transform.position += (Vector3)(Random.insideUnitCircle * 0.3f);
            }
        }
    }

    private GameObject FindClosestPrankTarget()
    {
        GameObject closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (var target in prankTargets)
        {
            if (target == null) continue;

            float distance = Vector3.Distance(companionTransform.position, target.transform.position);
            if (distance < prankRange && distance < closestDistance)
            {
                closest = target;
                closestDistance = distance;
            }
        }

        return closest;
    }

    public void SetPranksEnabled(bool enabled)
    {
        pranksEnabled = enabled;

        if (prankCoroutine != null)
        {
            StopCoroutine(prankCoroutine);
        }

        if (pranksEnabled)
        {
            prankCoroutine = StartCoroutine(PrankRoutine());
        }
    }
}
