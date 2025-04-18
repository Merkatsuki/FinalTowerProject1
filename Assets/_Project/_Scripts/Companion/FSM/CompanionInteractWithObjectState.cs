// CompanionInteractWithObjectState.cs
using UnityEngine;
using System.Collections.Generic;

public class CompanionInteractWithObjectState : CompanionState
{
    private InteractableBase target;
    private IRobotPerceivable perceivable;
    private List<RobotInteractionSO> interactions;
    private bool hasExecuted;

    public CompanionInteractWithObjectState(CompanionController companion, CompanionFSM fsm, InteractableBase target, IRobotPerceivable perceivable)
        : base(companion, fsm)
    {
        this.target = target;
        this.perceivable = perceivable;
        this.interactions = perceivable.GetRobotInteractions();
    }

    public override void OnEnter()
    {
        companion.LockInteraction();
        hasExecuted = true;

        foreach (var interaction in interactions)
        {
            interaction.OnEnter(companion, target);
            interaction.Execute(companion, target);
        }
    }

    public override void Tick()
    {
        if (!hasExecuted) return;

        bool allReady = true;
        foreach (var interaction in interactions)
        {
            if (!interaction.ShouldExit(companion, target))
            {
                allReady = false;
                break;
            }
        }

        if (allReady)
        {
            companion.UnlockInteraction();
            if (perceivable is CompanionClueInteractable clue)
            {
                clue.MarkHandled();
            }

            companion.Perception.MarkAsHandled(perceivable);
            fsm.ChangeState(companion.idleState);
        }
    }
}
