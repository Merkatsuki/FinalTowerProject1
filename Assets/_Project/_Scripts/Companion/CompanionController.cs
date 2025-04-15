using UnityEngine;

public class CompanionController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public float followDistance = 2.5f;

    private IPerceivable currentTarget;
    private CompanionFSM fsm;

    // State instances
    public CompanionFollowState followState;
    public CompanionIdleState idleState;

    void Awake()
    {
        fsm = new CompanionFSM();
        followState = new CompanionFollowState(this, fsm);
        idleState = new CompanionIdleState(this, fsm);
    }

    void Start()
    {
        fsm.Initialize(idleState);
    }

    void Update()
    {
        fsm.Tick();

        // TEMP: Manual triggers for testing
        if (Input.GetKeyDown(KeyCode.I))
            fsm.ChangeState(idleState);
        if (Input.GetKeyDown(KeyCode.F))
            fsm.ChangeState(followState);
    }

    void FixedUpdate()
    {
        fsm.FixedTick();
    }

    public void SetTargetToInvestigate(IPerceivable target)
    {
        if (currentTarget != target)
        {
            currentTarget = target;
            fsm.ChangeState(GetInvestigateState(target));
        }
    }

    public void ClearInvestigationTarget()
    {
        if (currentTarget != null)
        {
            currentTarget = null;
            fsm.ChangeState(followState);
        }
    }

    public CompanionState GetInvestigateState(IPerceivable target)
    {
        return new CompanionInvestigateState(this, fsm, target);
    }

    public void MoveTo(Vector2 target)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, 3f * Time.deltaTime);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (player != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, followDistance);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, player.position);
        }

        if (Application.isPlaying && fsm != null)
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, fsm.GetCurrentStateName());
        }
    }
#endif
}