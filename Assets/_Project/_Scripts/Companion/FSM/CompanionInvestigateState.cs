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

    public override void Tick()
    {
        if (target == null || !target.IsAvailable())
        {
            fsm.ChangeState(companion.idleState);
            return;
        }

        float dist = Vector2.Distance(companion.transform.position, targetTransform.position);
        if (dist <= arrivalThreshold)
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
        else
        {
            companion.MoveTo(targetTransform.position);
        }
    }
}