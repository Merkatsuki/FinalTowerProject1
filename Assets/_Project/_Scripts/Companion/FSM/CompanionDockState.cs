using UnityEngine;
using UnityEngine.AI;

public class CompanionDockState : CompanionState
{
    private DockConfig config;
    private NavMeshAgent agent;
    private float arrivalThreshold = 0.1f;
    private bool arrived;
    private float timer;

    public CompanionDockState(CompanionController companion, CompanionFSM fsm, DockConfig config)
        : base(companion, fsm)
    {
        this.config = config;
        this.agent = companion.GetComponent<NavMeshAgent>();
    }

    public override CompanionStateType StateType => CompanionStateType.Docking;

    public override void OnEnter()
    {
        arrived = false;
        timer = 0f;
        agent.SetDestination(config.Position);
        Debug.Log("[DockState] Moving to dock position: " + config.Position);
    }

    public override void Tick()
    {
        if (arrived)
        {
            if (config.stayDockedIndefinitely)
            {
                return;
            }

            timer += Time.deltaTime;
            if (timer >= config.HoverTime)
            {
                Debug.Log("[DockState] Dock complete. Returning to follow.");
                config.OnComplete?.Invoke();
                fsm.ResumeDefault(companion);
            }
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= arrivalThreshold)
        {
            Debug.Log("[DockState] Reached dock position.");
            arrived = true;
            agent.ResetPath();
        }
    }

    public override void OnExit()
    {
        agent.ResetPath();
    }
}

public class DockConfig
{
    public Vector3 Position;
    public float HoverTime;
    public bool stayDockedIndefinitely;
    public System.Action OnComplete;

    public DockConfig(Vector3 position, float hoverTime = 1.5f, System.Action onComplete = null, bool stayDocked = false)
    {
        Position = position;
        HoverTime = hoverTime;
        OnComplete = onComplete;
        stayDockedIndefinitely = stayDocked;
    }
}