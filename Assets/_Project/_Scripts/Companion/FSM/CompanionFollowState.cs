using UnityEngine;

public class CompanionFollowState : CompanionState
{
    public CompanionFollowState(CompanionController companion, CompanionFSM fsm) : base(companion, fsm) { }

    public override void Tick()
    {
        if (companion.TryAutoInvestigate()) return;

        float dist = Vector2.Distance(companion.transform.position, companion.player.position);
        if (dist > companion.followDistance)
        {
            companion.MoveTo(companion.player.position);
        }
        else
        {
            fsm.ChangeState(companion.idleState);
        }
    }
}