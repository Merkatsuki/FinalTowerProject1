using UnityEngine;

public class CompanionInvestigateState : CompanionState
{
    private IWorldInteractable target;
    private const float arrivalDistance = 0.5f;

    public CompanionInvestigateState(CompanionController companion, CompanionFSM fsm, IWorldInteractable target)
        : base(companion, fsm)
    {
        this.target = target;
    }

    public override void OnEnter()
    {
        if (target == null)
        {
            fsm.ChangeState(new CompanionIdleState(companion, fsm));
            return;
        }

        if (target.GetTransform().TryGetComponent<IHoverProfileProvider>(out var provider))
        {
            var profile = provider.GetHoverProfile();
            if (profile != null)
            {
                companion.flightController.SetTargetWithHoverProfile(target.GetTransform().position, profile);
                return;
            }
        }

        // fallback
        companion.flightController.SetTarget(target.GetTransform().position);
    }

    public override void Tick()
    {
        if (target == null)
        {
            fsm.ChangeState(new CompanionIdleState(companion, fsm));
            return;
        }

        if (companion.flightController.ReachedTarget(arrivalDistance))
        {
            Debug.Log($"[Companion] Reached target: {target.GetDisplayName()}");
            fsm.ChangeState(new CompanionInteractWithObjectState(companion, fsm, target));
        }
    }

    public override void OnExit()
    {
        companion.flightController.allowDefaultFollow = false;
    }

    public override CompanionStateType StateType => CompanionStateType.Investigate;
}