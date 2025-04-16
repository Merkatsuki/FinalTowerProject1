using UnityEngine;

public class CompanionInvestigateState : CompanionState
{
    private IPerceivable target;
    private Transform targetTransform;
    private float arrivalThreshold = 0.5f;

    public CompanionInvestigateState(CompanionController companion, CompanionFSM fsm, IPerceivable target)
        : base(companion, fsm)
    {
        this.target = target;
        this.targetTransform = target.GetTransform();
    }

    public override void OnEnter()
    {
        if (!IsValidTarget()) return;

        companion.flightController.SetTargetWithHoverProfile(
            targetTransform.position,
            companion.investigateHoverProfile
        );
    }

    public override void Tick()
    {
        if (!IsValidTarget()) return;

        if (companion.flightController.ReachedTarget(arrivalThreshold))
        {
            TransitionToInteractionOrIdle();
        }
    }

    public override void OnExit()
    {
        companion.flightController.ClearTarget();
    }

    private bool IsValidTarget()
    {
        if (target == null || !target.IsAvailable())
        {
            fsm.ChangeState(companion.idleState);
            return false;
        }
        return true;
    }

    private void TransitionToInteractionOrIdle()
    {
        if (targetTransform.TryGetComponent(out CompanionClueInteractable clue))
        {
            fsm.ChangeState(new CompanionInteractWithObjectState(companion, fsm, clue, target));
        }
        else
        {
            companion.Perception.MarkAsHandled(target);
            fsm.ChangeState(companion.idleState);
        }
    }
}