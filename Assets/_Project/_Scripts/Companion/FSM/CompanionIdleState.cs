using Momentum;
using UnityEngine;
using UnityEngine.AI;

public class CompanionIdleState : CompanionState
{
    private NavMeshAgent agent;

    public CompanionIdleState(CompanionController companion, CompanionFSM fsm)
        : base(companion, fsm)
    {
        agent = companion.GetComponent<NavMeshAgent>();
    }

    public override CompanionStateType StateType => CompanionStateType.Idle;

    public override void OnEnter()
    {
        agent.ResetPath();
    }

    public override void Tick()
    {
        if (companion.defaultFollowTarget == null)
            return;

        float distance = Vector2.Distance(
            companion.transform.position,
            companion.defaultFollowTarget.position
        );

        if (!InputManager.instance.IsCommandMode && distance > companion.followDistance)
        {
            fsm.ChangeState(companion.followState);
        }
    }

    public override void OnExit()
    {
        agent.ResetPath();
    }
}
