using UnityEngine;
using UnityEngine.AI;

public class CompanionWaitHereState : CompanionState
{
    private NavMeshAgent agent;

    public CompanionWaitHereState(CompanionController companion, CompanionFSM fsm)
        : base(companion, fsm)
    {
        this.agent = companion.GetComponent<NavMeshAgent>();
    }

    public override CompanionStateType StateType => CompanionStateType.WaitHere;

    public override void OnEnter()
    {
        agent.ResetPath();
        Debug.Log("[Companion] Now waiting in place.");
    }

    public override void Tick()
    {
        // Optional: add emotion-aware idle effects or timed comments
    }

    public override void OnExit()
    {
        agent.ResetPath();
        Debug.Log("[Companion] Exiting WaitHere state.");
    }
}
