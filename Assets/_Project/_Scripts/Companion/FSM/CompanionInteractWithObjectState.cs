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

    public override void OnEnter()
    {
        if (target == null)
        {
            fsm.ChangeState(new CompanionIdleState(companion, fsm));
            return;
        }

        Debug.Log($"[Companion] Interacting with: {target.GetDisplayName()}");
        target.OnInteract(companion);
    }

    public override void Tick()
    {
        waitTimer += Time.deltaTime;

        foreach (var strategy in target.GetExitStrategies())
        {
            if (strategy != null && strategy.ShouldExit(companion, target))
            {
                Debug.Log("[Companion] Exit strategy met, returning to Idle.");
                fsm.ChangeState(new CompanionIdleState(companion, fsm));
                return;
            }
        }

        if (waitTimer >= maxWaitTime)
        {
            Debug.LogWarning("[Companion] Max interaction time reached. Forcing return to Idle.");
            fsm.ChangeState(new CompanionIdleState(companion, fsm));
        }
    }

    public override CompanionStateType StateType => CompanionStateType.InteractWithObject;
}