using UnityEngine;

public class CompanionReturnToPlayerState : CompanionState
{
    public CompanionReturnToPlayerState(CompanionController companion, CompanionFSM fsm) : base(companion, fsm) { }

    public override void Tick()
    {
        Vector2 target = companion.player.position;
        float dist = Vector2.Distance(companion.transform.position, target);

        if (dist <= companion.followDistance)
        {
            fsm.ChangeState(companion.idleState);
        }
        else
        {
            companion.MoveTo(target);
        }
    }
}