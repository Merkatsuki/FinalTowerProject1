using UnityEngine;
using UnityEngine.AI;

public class CompanionFollowState : CompanionState
{
    private NavMeshAgent agent;

    public CompanionFollowState(CompanionController companion, CompanionFSM fsm)
        : base(companion, fsm)
    {
        agent = companion.GetComponent<NavMeshAgent>();
    }

    public override CompanionStateType StateType => CompanionStateType.Follow;

    public override void OnEnter()
    {
        if (companion.defaultFollowTarget != null)
        {
            agent.SetDestination(companion.defaultFollowTarget.position);
        }
    }

    public override void Tick()
    {
        if (companion.defaultFollowTarget == null)
            return;

        float distance = Vector2.Distance(
            companion.transform.position,
            companion.defaultFollowTarget.position
        );

        if (distance > companion.followDistance)
        {
            agent.SetDestination(companion.defaultFollowTarget.position);
        }
        else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            fsm.ChangeState(companion.idleState);
        }
    }

    public override void OnExit()
    {
        agent.ResetPath();
    }
}
