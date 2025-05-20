// CompanionController.cs - IPuzzleInteractor implementation with FSM, NavMesh, emotion, and interactable coordination
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;
using Momentum;
using UnityEngine.AI;
using System;

public class CompanionController : MonoBehaviour, IPuzzleInteractor
{
    #region Serialized Fields

    [Header("Flight & Perception")]
    public Transform defaultFollowTarget;
    public float followDistance = .5f;

    [Header("FSM & States")]
    public CompanionFSM fsm { get; private set; }
    public CompanionFollowState followState;
    public CompanionIdleState idleState;
    public CompanionMoveToPointState moveToPointState;
    public CompanionWaitHereState waitHereState { get; private set; }

    [Header("Emotion & Visuals")]
    [SerializeField] private Light2D robotGlowLight;
    [SerializeField] private float chargedGlowIntensity = 1.2f;
    [SerializeField] private float glowFadeDuration = 1f;
    [SerializeField] private CompanionStatusUI statusUI;

    [Header("Cooldown & Lock")]
    [SerializeField] private float retargetCooldown = 0.2f;

    [Header("Zones")]
    [SerializeField] private Transform hubSpawnPosition;

    #endregion

    #region Private Fields

    private float retargetCooldownTimer = 0f;
    private Coroutine emotionGlowCoroutine;
    private bool interactionLocked = false;
    private IWorldInteractable currentTarget;
    private IWorldInteractable playerCommandTarget;

    #endregion

    #region Component Accessors

    public CompanionVisualController VisualController { get; private set; }
    public CompanionPerception Perception { get; private set; }
    public NavMeshAgent Agent { get; private set; }

    #endregion

    #region Target & Interaction State

    public TargetData CurrentTarget { get; private set; }
    public void SetCurrentTarget(TargetData data) => CurrentTarget = data;
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

    public EmotionTag GetEmotion() => EmotionSwitcher.Instance?.GetCurrentEmotion() ?? EmotionTag.Neutral;
    public void SetEmotion(EmotionTag emotion) => currentEmotion = emotion;

    public bool WasCommanded(IWorldInteractable target) => playerCommandTarget == target;
    public bool HasPendingPlayerCommand() => playerCommandTarget != null;
    public void ClearPlayerCommand() => playerCommandTarget = null;

    #endregion

    #region IPuzzleInteractor Implementation

    public GameObject GetInteractorObject() => gameObject;
    public string GetDisplayName() => "Companion";

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        InitializeComponents();
    }

    private void Start()
    {
        if (ZoneManager.Instance != null)
            ZoneManager.Instance.OnPlayerZoneChanged += HandleZoneChange;
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
        if (VisualController != null && Agent != null)
            VisualController.UpdateVisuals(Agent.velocity);
        ApplyWindZones();
    }

    #endregion

    #region Initialization

    private void InitializeComponents()
    {
        fsm = new CompanionFSM();
        Agent = GetComponent<NavMeshAgent>();
        Perception = GetComponent<CompanionPerception>();
        VisualController = GetComponent<CompanionVisualController>();
        followState = new CompanionFollowState(this, fsm);
        idleState = new CompanionIdleState(this, fsm);
        moveToPointState = new CompanionMoveToPointState(this, fsm);
        waitHereState = new CompanionWaitHereState(this, fsm);
        fsm.Initialize(followState, statusUI);
    }

    #endregion

    #region Player Command Entry Points

    public void CommandMoveToPoint(Vector2 worldPosition)
    {
        SetCurrentTarget(new TargetData(worldPosition));
        fsm.ChangeState(moveToPointState);
    }

    public void CommandWaitHere()
    {
        fsm.PushState(waitHereState);
    }

    public void CommandResume()
    {
        fsm.PopState();
    }

    public void IssuePlayerCommand(IWorldInteractable target)
    {
        playerCommandTarget = target;
        SetCurrentTarget(new TargetData(target.GetTransform().position, target));
        fsm.ChangeState(new CompanionInvestigateState(this, fsm, target));
    }

    public void DockTo(DockConfig config)
    {
        fsm.ChangeState(new CompanionDockState(this, fsm, config));
    }

    private void HandleZoneChange(ZoneTag zone)
    {
        if (zone == ZoneTag.Hub && hubSpawnPosition != null)
        {
            Debug.Log("[CompanionController] Player entered Hub — switching to HubState.");
            fsm.ChangeState(new CompanionHubState(this, fsm, hubSpawnPosition.position));
        }
        else if (ZoneManager.Instance.GetPlayerZone() != ZoneTag.Hub && fsm.CurrentStateType == CompanionStateType.Hub)
        {
            // Exiting the hub → return to follow state and reposition
            Debug.Log("[CompanionController] Player left Hub — teleporting companion to follow target.");

            // 1. Teleport near follow target
            if (defaultFollowTarget != null)
            {
                Agent.enabled = false;
                transform.position = defaultFollowTarget.position;
                transform.rotation = Quaternion.identity;
                Agent.enabled = true;
                Agent.isStopped = false;
            }

            // 2. Resume follow
            fsm.ChangeState(followState);
        }
    }


    #endregion

    #region Emotion Visual Feedback

    [SerializeField] private EmotionTag currentEmotion = EmotionTag.Neutral;

    public void DisplayEmotionCharge(EmotionTag emotion, float duration = 2f)
    {
        Color color = EmotionColorMap.GetColor(emotion);

        if (robotGlowLight != null)
        {
            if (emotionGlowCoroutine != null)
                StopCoroutine(emotionGlowCoroutine);

            emotionGlowCoroutine = StartCoroutine(GlowCoroutine(color, duration));
        }
    }

    public void ClearEmotionVisual(bool skipFade = false)
    {
        if (robotGlowLight != null)
        {
            if (skipFade)
                robotGlowLight.intensity = 0f;
            else if (emotionGlowCoroutine != null)
                StopCoroutine(emotionGlowCoroutine);
        }
    }

    private IEnumerator GlowCoroutine(Color color, float duration)
    {
        float from = robotGlowLight.intensity;
        float elapsed = 0f;
        robotGlowLight.color = color;

        while (elapsed < duration)
        {
            robotGlowLight.intensity = Mathf.Lerp(from, chargedGlowIntensity, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        robotGlowLight.intensity = chargedGlowIntensity;
        yield return new WaitForSeconds(duration);
        robotGlowLight.intensity = 0f;
    }

    #endregion


    private void ApplyWindZones()
    {
        Vector2 totalWindForce = Vector2.zero;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.2f);
        foreach (var col in hits)
        {
            WindZone2D wind = col.GetComponent<WindZone2D>();
            if (wind != null && wind.IsActive())
            {
                totalWindForce += wind.GetWindForce();
            }
        }

        if (totalWindForce != Vector2.zero)
        {
            Agent.nextPosition += (Vector3)(totalWindForce * Time.fixedDeltaTime);

            // Optional stop logic for strong gusts
            Agent.isStopped = totalWindForce.magnitude > Agent.speed * 2f;
        }
        else
        {
            Agent.isStopped = false;
        }
    }
}

[Serializable]
public class TargetData
{
    public Vector2 Position;
    public IWorldInteractable Target;

    public TargetData(Vector2 pos)
    {
        Position = pos;
        Target = null;
    }

    public TargetData(Vector2 pos, IWorldInteractable t)
    {
        Position = pos;
        Target = t;
    }
}
