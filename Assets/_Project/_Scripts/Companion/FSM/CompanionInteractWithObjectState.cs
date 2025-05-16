using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class CompanionInteractWithObjectState : CompanionState
{
    private IWorldInteractable target;
    private float waitTimer = 0f;
    private float maxWaitTime = 5f;
    private NavMeshAgent agent;

    public CompanionInteractWithObjectState(CompanionController companion, CompanionFSM fsm, IWorldInteractable target)
        : base(companion, fsm)
    {
        this.target = target;
        this.agent = companion.GetComponent<NavMeshAgent>();
    }

    public override CompanionStateType StateType => CompanionStateType.InteractWithObject;

    public override void OnEnter()
    {
        agent.ResetPath();

        if (target == null || !target.CanBeInteractedWith(companion))
        {
            Debug.LogWarning("[InteractWithObject] Invalid or blocked target.");
            fsm.ResumeDefault(companion);
            return;
        }

        Debug.Log($"[InteractWithObject] Beginning interaction with: {target.GetDisplayName()}");
        waitTimer = 0f;

        foreach (var strategy in target.GetExitStrategies())
        {
            strategy?.OnEnter(companion, target);
        }

        InteractWithTarget();
    }

    public override void Tick()
    {
        waitTimer += Time.deltaTime;

        foreach (var strategy in target.GetExitStrategies())
        {
            if (strategy != null && strategy.ShouldExit(companion, target))
            {
                Debug.Log("[InteractWithObject] Exit condition met.");
                fsm.ResumeDefault(companion);
                return;
            }
        }

        if (waitTimer >= maxWaitTime)
        {
            Debug.LogWarning("[InteractWithObject] Timeout. Returning to default.");
            fsm.ResumeDefault(companion);
        }
    }

    public override void OnExit()
    {
        agent.ResetPath();
    }

    private void InteractWithTarget()
    {
        if (target != null)
        {
            Debug.Log($"[CompanionInteractWithObjectState] Interacting with {target.GetDisplayName()}");

            target.OnInteract(companion);
            target.OnInteractionComplete(companion, true);

            companion.Perception.MarkAsHandled(target);
        }
    }
}