using UnityEngine;
using System.Collections.Generic;

public class CompanionInteractWithObjectState : CompanionState
{
    private CompanionClueInteractable target;
    private List<RobotInteractionSO> interactions;
    private bool hasExecuted;

    public CompanionInteractWithObjectState(CompanionController companion, CompanionFSM fsm, CompanionClueInteractable target)
        : base(companion, fsm)
    {
        this.target = target;
        this.interactions = target.GetRobotInteractions();
    }

    public override void OnEnter()
    {
        hasExecuted = true;
        companion.LockInteraction();

        foreach (var interaction in interactions)
        {
            interaction.OnEnter(companion, target);
            interaction.Execute(companion, target);
        }
    }

    public override void Tick()
    {
        if (!hasExecuted) return;

        bool allReady = true;
        foreach (var interaction in interactions)
        {
            if (!interaction.ShouldExit(companion, target))
            {
                allReady = false;
                break;
            }
        }

        if (allReady)
        {
            target.MarkHandled();
            companion.Perception.MarkAsHandled(target);
            companion.ClearCurrentTarget();
            companion.StartRetargetCooldown();
            companion.UnlockInteraction();
            fsm.ChangeState(companion.idleState);
        }
    }
}
