using UnityEngine;

public class CompanionInteractWithObjectState : CompanionState
{
    private CompanionClueInteractable target;
    private IPerceivable perceivable;
    private float delayTimer = 1.5f;
    private bool hasExecuted = false;

    public CompanionInteractWithObjectState(CompanionController companion, CompanionFSM fsm, CompanionClueInteractable target, IPerceivable perceivable)
        : base(companion, fsm)
    {
        this.target = target;
        this.perceivable = perceivable;
    }

    public override void Tick()
    {
        if (!hasExecuted)
        {
            foreach (var interaction in target.robotInteractions)
            {
                interaction.Execute(companion, target);
            }

            bool keepAvailable = false;
            foreach (var interaction in target.robotInteractions)
            {
                if (interaction.ShouldRemainAvailable())
                    keepAvailable = true;
            }

            if (!keepAvailable)
                companion.Perception.MarkAsHandled(perceivable);

            hasExecuted = true;
        }

        delayTimer -= Time.deltaTime;
        if (delayTimer <= 0f)
        {
            fsm.ChangeState(companion.idleState);
        }
    }
}