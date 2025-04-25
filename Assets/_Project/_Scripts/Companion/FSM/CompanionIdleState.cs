using UnityEngine;

public class CompanionIdleState : CompanionState
{
    public CompanionIdleState(CompanionController companion, CompanionFSM fsm)
        : base(companion, fsm)
    { }

    public override void Tick()
    {
        // Optional: idle visual effect or status UI here
    }

    public override void OnExit()
    {
        companion.flightController.allowDefaultFollow = false;
    }

    public override CompanionStateType StateType => CompanionStateType.Idle;
}