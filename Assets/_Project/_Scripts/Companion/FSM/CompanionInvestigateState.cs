using UnityEngine;

public class CompanionInvestigateState : CompanionState
{
    private IRobotPerceivable target;
    private Transform targetTransform;
    private float arrivalThreshold = 0.5f;

    public CompanionInvestigateState(CompanionController companion, CompanionFSM fsm, IRobotPerceivable target)
        : base(companion, fsm)
    {
        this.target = target;
        this.targetTransform = target.GetTransform();
    }

    public override void OnEnter()
    {
        Debug.Log($"Investigating: {target}");

        if (!IsValidTarget()) return;

        if (!targetTransform.TryGetComponent<IHoverProfileProvider>(out var provider))
        {
            Debug.LogWarning($"InvestigateState: Target '{targetTransform.name}' does not implement IHoverProfileProvider.");
            fsm.ChangeState(companion.idleState);
            return;
        }

        HoverStagingProfileSO profile = provider.GetHoverProfile();
        if (profile == null)
        {
            Debug.LogWarning($"InvestigateState: Target '{targetTransform.name}' did not provide a HoverStagingProfileSO.");
            fsm.ChangeState(companion.idleState);
            return;
        }

        companion.flightController.SetTargetWithHoverProfile(targetTransform.position, profile);
        // track active target during flight
    }

    public override void Tick()
    {
        if (!IsValidTarget()) return;

        if (companion.flightController.ReachedTarget(arrivalThreshold))
        {
            TransitionToInteractionOrIdle();
        }
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
            fsm.ChangeState(new CompanionInteractWithObjectState(companion, fsm, clue));
        }
        else
        {
            companion.Perception.MarkAsHandled(target);
            companion.ClearCurrentTarget();
            fsm.ChangeState(companion.idleState);
        }
    }
}
