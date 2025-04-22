using UnityEngine;

public class CompanionIdleState : CompanionState
{
    public CompanionIdleState(CompanionController companion, CompanionFSM fsm)
        : base(companion, fsm)
    { }

    public override void Tick()
    {
        if (!companion.IsInteractionLocked && companion.TryAutoInvestigate())
        {
            return;
        }

        // Follow default target if nothing else is happening
        if (companion.flightController.defaultFollowTarget != null)
        {
            companion.flightController.SetTarget(companion.flightController.defaultFollowTarget.position);
            fsm.ChangeState(new CompanionFollowState(companion, fsm));
        }
    }

    public override CompanionStateType StateType => CompanionStateType.Idle;
}