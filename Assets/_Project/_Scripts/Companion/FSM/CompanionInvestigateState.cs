using UnityEngine;

public partial class CompanionInvestigateState : CompanionState
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
        Debug.Log("Investigating: " + targetTransform.name);
    }

    public override void Tick()
    {
        Debug.DrawLine(companion.transform.position, targetTransform.position, Color.red);

        if (target == null || !target.IsAvailable())
        {
            Debug.Log("Target lost or unavailable.");
            fsm.ChangeState(companion.idleState); // Fallback
            return;
        }

        float dist = Vector2.Distance(companion.transform.position, targetTransform.position);

        if (dist <= arrivalThreshold)
        {
            Debug.Log("Reached target: " + targetTransform.name);

            if (targetTransform.TryGetComponent(out CompanionClueInteractable clue))
            {
                clue.RobotInteract(companion);
            }
            // You could transition to an Interact state here
            fsm.ChangeState(companion.idleState);
        }
        else
        {
            // Move toward the target using a flying approach (for now)
            Vector2 newPos = Vector2.MoveTowards(
                companion.transform.position,
                targetTransform.position,
                3f * Time.deltaTime
            );

            companion.transform.position = newPos;
        }
    }

    public override void OnExit()
    {
        Debug.Log("Exiting Investigate state.");
    }
}