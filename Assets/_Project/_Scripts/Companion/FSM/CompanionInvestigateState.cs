using UnityEngine;
using UnityEngine.AI;

public class CompanionInvestigateState : CompanionState
{
    private IWorldInteractable target;
    private const float arrivalDistance = 1f;
    private NavMeshAgent agent;

    public CompanionInvestigateState(CompanionController companion, CompanionFSM fsm, IWorldInteractable target)
        : base(companion, fsm)
    {
        this.target = target;
        this.agent = companion.GetComponent<NavMeshAgent>();
    }

    public override void OnEnter()
    {
        if (target == null)
        {
            fsm.ChangeState(companion.idleState);
            return;
        }

        Vector3 destination = target.GetTransform().position;

        agent.SetDestination(destination);
    }

    public override void Tick()
    {
        if (target == null)
        {
            fsm.ResumeDefault(companion);
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= arrivalDistance)
        {
            Debug.Log($"[Companion] Reached target: {target.GetDisplayName()}");
            fsm.ChangeState(new CompanionInteractWithObjectState(companion, fsm, target));
        }
    }

    public override void OnExit()
    {
        agent.ResetPath();
    }

    public override CompanionStateType StateType => CompanionStateType.Investigate;
}
