using UnityEngine;

public class CompanionIdleState : CompanionState
{
    public CompanionIdleState(CompanionController companion, CompanionFSM fsm) : base(companion, fsm) { }

    public override void OnEnter()
    {
        // Could trigger idle animation here
        Debug.Log("Companion entered IDLE state.");
    }

    public override void Tick()
    {
        // Maybe look at nearby objects or play idle animation
    }

    public override void OnExit()
    {
        Debug.Log("Companion exited IDLE state.");
    }
}
