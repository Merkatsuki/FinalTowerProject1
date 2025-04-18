// CompanionController.cs
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CompanionController : MonoBehaviour
{
    public float followDistance = 2.5f;
    [SerializeField] private EnergyType currentEnergy = EnergyType.None;
    [SerializeField] private Light2D robotGlowLight;
    [SerializeField] private float chargedGlowIntensity = 1.2f;

    public CompanionFSM fsm { get; private set; }
    public RobotFlightController flightController { get; private set; }
    public CompanionPerception Perception { get; private set; }
    public bool IsInteractionLocked { get; private set; } = false;

    public CompanionFollowState followState;
    public CompanionIdleState idleState;

    private void Awake()
    {
        InitializeComponents();
    }

    private void Start()
    {
        fsm.Initialize(idleState);
    }

    private void Update()
    {
        fsm.Tick();
    }

    private void FixedUpdate()
    {
        fsm.FixedTick();
    }

    private void InitializeComponents()
    {
        fsm = new CompanionFSM();
        flightController = GetComponent<RobotFlightController>();
        Perception = GetComponent<CompanionPerception>();
        followState = new CompanionFollowState(this, fsm);
        idleState = new CompanionIdleState(this, fsm);
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

    public void LockInteraction() => IsInteractionLocked = true;
    public void UnlockInteraction() => IsInteractionLocked = false;

    public void SetEnergyType(EnergyType type) => currentEnergy = type;
    public EnergyType GetEnergyType() => currentEnergy;

    public Light2D GetRobotLight() => robotGlowLight;

    public float GetChargedGlowIntensity() => chargedGlowIntensity;

    public IEnumerator ChargeGlow(Color color, float duration)
    {
        if (robotGlowLight == null) yield break;

        float from = robotGlowLight.intensity;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            robotGlowLight.intensity = Mathf.Lerp(from, chargedGlowIntensity, t);
            robotGlowLight.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }

        robotGlowLight.intensity = chargedGlowIntensity;
        robotGlowLight.color = color;
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