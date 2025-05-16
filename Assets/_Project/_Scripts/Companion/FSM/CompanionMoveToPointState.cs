using UnityEngine;
using UnityEngine.AI;

public class CompanionMoveToPointState : CompanionState
{
    private Vector2 _targetPosition;
    private float _arrivalThreshold = 0.15f;
    private bool _arrived;
    private NavMeshAgent agent;

    public CompanionMoveToPointState(CompanionController companion, CompanionFSM fsm) : base(companion, fsm)
    {
        agent = companion.GetComponent<NavMeshAgent>();
    }

    public override CompanionStateType StateType => CompanionStateType.MoveToPoint;

    public override void OnEnter()
    {
        var target = companion.CurrentTarget;
        _targetPosition = target.Position;
        _arrived = false;

        agent.SetDestination(_targetPosition);
    }

    public override void Tick()
    {
        if (_arrived || agent.pathPending)
            return;

        if (agent.remainingDistance <= _arrivalThreshold)
        {
            _arrived = true;
            fsm.ChangeState(companion.idleState);
        }
    }

    public override void OnExit()
    {
        agent.ResetPath();
    }
}