using UnityEngine;
using System.Collections.Generic;

public class CompanionInteractWithObjectState : CompanionState
{
    private IWorldInteractable target;
    private float waitTimer = 0f;
    private float maxWaitTime = 5f;

    public CompanionInteractWithObjectState(CompanionController companion, CompanionFSM fsm, IWorldInteractable target)
        : base(companion, fsm)
    {
        this.target = target;
    }

    public override CompanionStateType StateType => CompanionStateType.InteractWithObject;

    public override void OnEnter()
    {
        if (target == null || !target.CanBeInteractedWith(companion))
        {
            Debug.LogWarning("[InteractWithObject] Invalid or blocked target.");
            fsm.ChangeState(companion.idleState);
            return;
        }

        Debug.Log($"[InteractWithObject] Beginning interaction with: {target.GetDisplayName()}");
        waitTimer = 0f;

        foreach (var strategy in target.GetExitStrategies())
        {
            strategy?.OnEnter(companion, target);
        }

        Debug.Log($"[CompanionInteractWithObjectState] Entered. Target: {target?.GetDisplayName()}");
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
                fsm.ChangeState(companion.idleState);
                return;
            }
        }

        if (waitTimer >= maxWaitTime)
        {
            Debug.LogWarning("[InteractWithObject] Timeout. Returning to Idle.");
            fsm.ChangeState(companion.idleState);
        }
    }

    public override void OnExit()
    {
        companion.flightController.allowDefaultFollow = false;
    }

    private void InteractWithTarget()
    {
        if (target != null)
        {
            Debug.Log($"[CompanionInteractWithObjectState] Interacting with: {target.GetDisplayName()}");
            target.OnInteract(companion);
        }
        else
        {
            Debug.LogWarning("[CompanionInteractWithObjectState] No target to interact with.");
        }
    }
}
