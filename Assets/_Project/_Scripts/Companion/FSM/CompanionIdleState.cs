using UnityEngine;

public class CompanionIdleState : CompanionState
{
    public CompanionIdleState(CompanionController companion, CompanionFSM fsm) : base(companion, fsm) { }

    public override void OnEnter()
    {
        companion.flightController.ClearTarget();
    }

    public override void Tick()
    {
        if (!companion.IsInteractionLocked && companion.TryAutoInvestigate()) return;

        float distance = Vector2.Distance(
        companion.transform.position,
        companion.flightController.defaultFollowTarget.position
    );

        if (distance > companion.followDistance)
        {
            fsm.ChangeState(companion.followState);
        }
    }
}