using UnityEngine;

public class CompanionController : MonoBehaviour
{
    [Header("References")]
    public float followDistance = 2.5f;

    private CompanionFSM fsm;
    public CompanionFollowState followState;
    public CompanionIdleState idleState;
    public RobotFlightController flightController { get; private set; }
    public CompanionPerception Perception { get; private set; }

    void Awake()
    {
        fsm = new CompanionFSM();
        followState = new CompanionFollowState(this, fsm);
        idleState = new CompanionIdleState(this, fsm);
        Perception = GetComponent<CompanionPerception>();

        flightController = GetComponent<RobotFlightController>();
    }

    void Start()
    {
        fsm.Initialize(idleState);
    }

    void Update() => fsm.Tick();
    void FixedUpdate() => fsm.FixedTick();

    public bool TryAutoInvestigate()
    {
        var target = Perception.GetCurrentTarget();
        if (target != null)
        {
            fsm.ChangeState(new CompanionInvestigateState(this, fsm, target));
            return true;
        }
        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (flightController != null && flightController.defaultFollowTarget != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, followDistance);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, flightController.defaultFollowTarget.position);
        }

        if (Application.isPlaying && fsm != null)
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, fsm.GetCurrentStateName());
        }
    }
#endif
}
