using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider2D))]
public class CompanionWindEffect : MonoBehaviour
{
    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
            Debug.LogError("CompanionWindEffect: Could not find NavMeshAgent on Companion.");
    }

    void Update()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        foreach (var col in colliders)
        {
            WindZone2D wind = col.GetComponent<WindZone2D>();
            if (wind != null && wind.IsActive())
            {
                Vector3 offset = (Vector3)wind.GetWindForce() * Time.deltaTime;
                agent.nextPosition += offset;

                if (wind.force > agent.speed * 2f)
                    agent.isStopped = true;
                else
                    agent.isStopped = false;
            }
        }
    }
}
