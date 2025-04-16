using UnityEngine;

public class CompanionController : MonoBehaviour
{
    [Header("References")]
    public float followDistance = 2.5f;
    public HoverStagingProfileSO investigateHoverProfile;

    private CompanionFSM fsm;
    public CompanionFollowState followState;
    public CompanionIdleState idleState;
    public RobotFlightController flightController { get; private set; }
    public CompanionPerception Perception { get; private set; }

    private void Awake()
    {
        InitializeComponents();
    }

    private void Start()
    {
        fsm.Initialize(idleState);
    }

    private void Update() => fsm.Tick();
    private void FixedUpdate() => fsm.FixedTick();

    private void InitializeComponents()
    {
        fsm = new CompanionFSM();
        followState = new CompanionFollowState(this, fsm);
        idleState = new CompanionIdleState(this, fsm);
        Perception = GetComponent<CompanionPerception>();
        flightController = GetComponent<RobotFlightController>();
    }

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
        DrawFollowRangeGizmo();
        DrawStateLabel();
    }

    private void DrawFollowRangeGizmo()
    {
        if (flightController != null && flightController.defaultFollowTarget != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, followDistance);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, flightController.defaultFollowTarget.position);
        }
    }

    private void DrawStateLabel()
    {
        if (Application.isPlaying && fsm != null)
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, fsm.GetCurrentStateName());
        }
    }
#endif
}