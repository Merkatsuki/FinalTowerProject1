using UnityEngine;

public class CompanionMoveToPointState : CompanionState
{
    private Vector2 _targetPosition;
    private float _arrivalThreshold = 0.15f;
    private bool _arrived;

    public CompanionMoveToPointState(CompanionController companion, CompanionFSM fsm) : base(companion, fsm) { }

    public override CompanionStateType StateType => CompanionStateType.MoveToPoint;

    public override void OnEnter()
    {
        var target = companion.CurrentTarget;
        _targetPosition = target.Position;

        Debug.Log($"[MoveToPoint] Entering state. Target position: {_targetPosition}");

        _arrived = false;
        companion.flightController.SetTarget(_targetPosition);
    }

    public override void Tick()
    {
        base.Tick();
        float dist = Vector2.Distance(companion.transform.position, _targetPosition);
        Debug.Log($"[MoveToPoint] Distance to target: {dist}");

        if (!_arrived && dist < _arrivalThreshold)
        {
            _arrived = true;
            Debug.Log("[MoveToPoint] Arrived at target. Switching to Idle.");
            companion.fsm.ChangeState(companion.idleState);
        }
    }

    public override void OnExit()
    {
        companion.flightController.allowDefaultFollow = false;

        Debug.Log("[MoveToPoint] Exiting state. Clearing target.");
        companion.flightController.ClearTarget();
    }
}


