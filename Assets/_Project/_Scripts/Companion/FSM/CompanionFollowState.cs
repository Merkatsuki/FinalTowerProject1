using UnityEngine;

public class CompanionFollowState : CompanionState
{
    public CompanionFollowState(CompanionController companion, CompanionFSM fsm) : base(companion, fsm) { }

    public override void Tick()
    {
        if (!companion.IsInteractionLocked && companion.TryAutoInvestigate()) return;

        if (companion.flightController.defaultFollowTarget == null)
        {
            Debug.LogWarning("No defaultFollowTarget set.");
            return;
        }

        float dist = Vector2.Distance(
            companion.transform.position,
            companion.flightController.defaultFollowTarget.position
        );

        if (dist > companion.followDistance)
        {
            companion.flightController.SetTarget(companion.flightController.defaultFollowTarget.position);
        }
        else
        {
            fsm.ChangeState(companion.idleState);
        }
    }

    public override void OnExit()
    {
        companion.flightController.ClearTarget();
    }

    public override CompanionStateType StateType => CompanionStateType.Follow;
}