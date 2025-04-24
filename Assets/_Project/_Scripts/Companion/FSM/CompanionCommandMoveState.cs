using UnityEngine;

public class CompanionCommandMoveState : CompanionState
{
    private Vector2 targetPoint;
    private const float arrivalThreshold = 0.2f;

    public CompanionCommandMoveState(CompanionController companion, CompanionFSM fsm, Vector2 point)
        : base(companion, fsm)
    {
        this.targetPoint = point;
    }

    public override void OnEnter()
    {
        Debug.Log($"[Companion] Moving to commanded point: {targetPoint}");
        companion.flightController.SetTarget(targetPoint);
    }

    public override void Tick()
    {
        if (companion.flightController.ReachedTarget(arrivalThreshold))
        {
            Debug.Log($"[Companion] Reached commanded point: {targetPoint}");
            fsm.ChangeState(companion.idleState);
        }
    }

    public override void OnExit()
    {
        companion.flightController.ClearTarget();
    }

    public override CompanionStateType StateType => CompanionStateType.CommandMove;
}


