using UnityEngine;

public class CompanionGlitchState : CompanionState
{
    private float glitchDuration = 2.5f;
    private float timer;

    public CompanionGlitchState(CompanionController companion, CompanionFSM fsm) : base(companion, fsm) { }

    public override void OnEnter()
    {
        timer = glitchDuration;
        Debug.Log("Companion is glitching!");
        // Trigger glitch visuals/sounds
    }

    public override void Tick()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            fsm.ChangeState(companion.idleState);
        }
        // Optional: erratic movement here
    }
}