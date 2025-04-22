using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;
using System.Collections.Generic;

public class CompanionController : MonoBehaviour, IPuzzleInteractor
{
    [Header("Flight & Perception")]
    public float followDistance = 2.5f;
    public RobotFlightController flightController { get; private set; }
    public CompanionPerception Perception { get; private set; }

    [Header("FSM & States")]
    public CompanionFSM fsm { get; private set; }
    public CompanionFollowState followState;
    public CompanionIdleState idleState;

    [Header("Status & Energy")]
    [SerializeField] private EnergyStateComponent energyState;
    [SerializeField] private Light2D robotGlowLight;
    [SerializeField] private float chargedGlowIntensity = 1.2f;
    [SerializeField] private float energyDuration = 10f;
    [SerializeField] private CompanionStatusUI statusUI;

    [Header("Cooldown & Lock")]
    [SerializeField] private float retargetCooldown = 0.2f;
    private float retargetCooldownTimer = 0f;
    private Coroutine energyTimerCoroutine;
    private bool interactionLocked = false;

    [Header("Emotion")]
    [SerializeField] private EmotionType currentEmotion = EmotionType.Neutral;

    // Interaction targets
    private IWorldInteractable currentTarget;
    private IWorldInteractable playerCommandTarget;

    public void SetCurrentTarget(IWorldInteractable target) => currentTarget = target;
    public void ClearCurrentTarget() => currentTarget = null;
    public IWorldInteractable GetCurrentTrackedTarget() => currentTarget;

    public bool IsBusy => currentTarget != null || IsInteractionLocked;
    public bool CanInvestigate() => !IsBusy && retargetCooldownTimer <= 0f;
    public void StartRetargetCooldown() => retargetCooldownTimer = retargetCooldown;

    public bool IsInteractionLocked => interactionLocked;
    public void LockInteraction() => interactionLocked = true;
    public void UnlockInteraction() => interactionLocked = false;

    public CompanionFSM GetFSM() => fsm;
    public CompanionPerception GetPerception() => Perception;

    public EmotionType GetEmotion() => currentEmotion;
    public void SetEmotion(EmotionType emotion) => currentEmotion = emotion;

    public void IssuePlayerCommand(IWorldInteractable target) => playerCommandTarget = target;
    public bool WasCommanded(IWorldInteractable target) => playerCommandTarget == target;
    public bool HasPendingPlayerCommand() => playerCommandTarget != null;
    public void ClearPlayerCommand() => playerCommandTarget = null;

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
        fsm.Initialize(idleState, statusUI);
    }

    #endregion

    #region Auto Investigate

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

    #region Energy Management

    public EnergyType GetEnergyType() => energyState?.GetEnergy() ?? EnergyType.None;
    public GameObject GetInteractorObject() => gameObject;
    public string GetDisplayName() => "Companion";

    public void SetEnergyType(EnergyType type)
    {
        if (energyState != null)
            energyState.SetEnergy(type);

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
        if (energyState != null)
            energyState.SetEnergy(EnergyType.None);

        if (robotGlowLight != null && skipFade)
            robotGlowLight.intensity = 0f;
    }

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

    #region Editor Debug

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

    #endregion
}