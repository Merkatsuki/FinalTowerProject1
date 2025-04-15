using UnityEngine;

public class CompanionInteractWithObjectState : CompanionState
{
    private InteractableBase target;
    private float interactionDelay = 1.0f;
    private float timer;

    public CompanionInteractWithObjectState(CompanionController companion, CompanionFSM fsm, InteractableBase target) : base(companion, fsm)
    {
        this.target = target;
    }

    public override void OnEnter()
    {
        timer = interactionDelay;
        Debug.Log("Starting interaction with: " + target.name);
    }

    public override void Tick()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            if (target is CompanionClueInteractable clue)
            {
                clue.RobotInteract(companion);
            }
            else
            {
                Debug.LogWarning($"Tried to interact with {target.name}, but it's not a CompanionClueInteractable.");
            }

            fsm.ChangeState(companion.idleState);
        }
    }
}