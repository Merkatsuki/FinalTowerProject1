using UnityEngine.Rendering.Universal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#region Core Declaration
public class CompanionController : MonoBehaviour
{
    public float followDistance = 2.5f;
    [SerializeField] private EnergyType currentEnergy = EnergyType.None;
    [SerializeField] private Light2D robotGlowLight;
    [SerializeField] private float chargedGlowIntensity = 1.2f;
    [SerializeField] private float energyDuration = 10f;

    [SerializeField] private float retargetCooldown = 2.1f;  //Right now set to just longer than charge on EnergyDockingZone to avoid retargetting while charging.  Might make this more reaonable later.
    private float retargetCooldownTimer = 0f;

    [SerializeField] private CompanionStatusUI statusUI;
    [SerializeField] private EmotionType currentEmotion = EmotionType.Neutral;


    public CompanionFSM fsm { get; private set; }
    public RobotFlightController flightController { get; private set; }
    public CompanionPerception Perception { get; private set; }
    public bool IsInteractionLocked { get; private set; } = false;

    public CompanionFollowState followState;
    public CompanionIdleState idleState;

    private Coroutine energyTimerCoroutine;

    private IRobotPerceivable currentTarget;
    public void SetCurrentTarget(IRobotPerceivable target) => currentTarget = target;
    public void ClearCurrentTarget() => currentTarget = null;
    public IRobotPerceivable GetCurrentTrackedTarget() => currentTarget;

    public bool IsBusy => currentTarget != null || IsInteractionLocked;
    public bool CanInvestigate() => !IsBusy && retargetCooldownTimer <= 0f;
    public void StartRetargetCooldown() => retargetCooldownTimer = retargetCooldown;


    // 🧩 Required by Entry/Exit Strategies & Debug

    private CompanionClueInteractable playerCommandTarget;
    private Dictionary<CompanionClueInteractable, float> lastSeenTimes = new();

    public CompanionFSM GetFSM() => fsm;
    public CompanionPerception GetPerception() => Perception;

    public EmotionType GetEmotion() => currentEmotion;
    public void SetEmotion(EmotionType emotion) => currentEmotion = emotion;

    public void IssuePlayerCommand(CompanionClueInteractable target) => playerCommandTarget = target;
    public bool WasCommanded(CompanionClueInteractable target) => playerCommandTarget == target;
    public bool HasPendingPlayerCommand() => playerCommandTarget != null;
    public void ClearPlayerCommand() => playerCommandTarget = null;

    public void RecordPerceptionTime(CompanionClueInteractable clue) => lastSeenTimes[clue] = Time.time;
    public float GetLastSeenTime(CompanionClueInteractable clue) => lastSeenTimes.TryGetValue(clue, out var time) ? time : float.MinValue;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        InitializeComponents();
    }

    private void Update()
    {
        fsm.Tick();

        if (retargetCooldownTimer > 0f)
            retargetCooldownTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        fsm.FixedTick();
    }

    #endregion

    #region Initialization
    private void InitializeComponents()
    {
        fsm = new CompanionFSM();
        flightController = GetComponent<RobotFlightController>();
        Perception = GetComponent<CompanionPerception>();
        followState = new CompanionFollowState(this, fsm);
        idleState = new CompanionIdleState(this, fsm);

        // Inject the FSM and the status UI reference
        fsm.Initialize(idleState, statusUI);
    }

    #endregion

    #region Investigation Logic
    public bool TryAutoInvestigate()
    {
        if (!CanInvestigate()) return false;

        var target = Perception.GetCurrentTarget();
        if (target != null)
        {
            currentTarget = target;
            fsm.ChangeState(new CompanionInvestigateState(this, fsm, target));
            return true;
        }
        return false;
    }

    #endregion

    #region Interaction Locking
    public void LockInteraction() => IsInteractionLocked = true;
    public void UnlockInteraction() => IsInteractionLocked = false;

    #endregion

    #region Energy System
    public void SetEnergyType(EnergyType type)
    {
        currentEnergy = type;
        if (energyTimerCoroutine != null)
            StopCoroutine(energyTimerCoroutine);

        if (type != EnergyType.None)
            energyTimerCoroutine = StartCoroutine(EnergyDecay());
    }

    private IEnumerator EnergyDecay()
    {
        float from = robotGlowLight.intensity;
        float elapsed = 0f;

        while (elapsed < energyDuration)
        {
            robotGlowLight.intensity = Mathf.Lerp(from, 0f, elapsed / energyDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        ClearEnergy(true);
    }

    public void ClearEnergy(bool skipFade = false)
    {
        currentEnergy = EnergyType.None;

        if (robotGlowLight != null && skipFade)
            robotGlowLight.intensity = 0f;
    }

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

    #endregion

    #region Gizmos & Editor Debug
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

#endregion
