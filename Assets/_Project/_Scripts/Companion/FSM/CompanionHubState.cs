using UnityEngine;
using UnityEngine.AI;

public class CompanionHubState : CompanionState
{
    private Vector3 hubPosition;
    private NavMeshAgent agent;

    public CompanionHubState(CompanionController companion, CompanionFSM fsm, Vector3 hubPosition)
        : base(companion, fsm)
    {
        this.hubPosition = hubPosition;
        this.agent = companion.GetComponent<NavMeshAgent>();
    }

    public override CompanionStateType StateType => CompanionStateType.Hub;

    public override void OnEnter()
    {
        agent.ResetPath();
        agent.enabled = false; // <- disable before teleporting

        companion.transform.position = hubPosition;
        companion.transform.rotation = Quaternion.identity; // optional

        agent.enabled = true;  // <- re-enable after teleporting
        agent.isStopped = true;

        Debug.Log("[CompanionHubState] Companion teleported to hub at: " + hubPosition);
        companion.LockInteraction();
    }

    public override void Tick()
    {
        // Do nothing
    }

    public override void OnExit()
    {
        agent.isStopped = false;
        companion.UnlockInteraction();
    }
}
