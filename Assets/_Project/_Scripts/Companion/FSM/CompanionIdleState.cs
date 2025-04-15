using UnityEngine;

public class CompanionIdleState : CompanionState
{
    public CompanionIdleState(CompanionController companion, CompanionFSM fsm) : base(companion, fsm) { }

    public override void Tick()
    {
        if (companion.TryAutoInvestigate()) return;

        float distance = Vector2.Distance(companion.transform.position, companion.player.position);
        if (distance > companion.followDistance)
        {
            fsm.ChangeState(companion.followState);
        }
    }
}