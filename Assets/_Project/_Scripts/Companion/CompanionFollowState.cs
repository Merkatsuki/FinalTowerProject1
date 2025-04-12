using UnityEngine;

public class CompanionFollowState : CompanionState
{
    public CompanionFollowState(CompanionController companion, CompanionFSM fsm) : base(companion, fsm) { }

    public override void Tick()
    {
        float dist = Vector2.Distance(companion.transform.position, companion.player.position);
        if (dist > companion.followDistance)
        {
            Vector2 target = companion.player.position;
            companion.transform.position = Vector2.MoveTowards(companion.transform.position, target, 3f * Time.deltaTime);
        }
    }
}
