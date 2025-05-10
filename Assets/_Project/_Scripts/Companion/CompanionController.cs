// CompanionController.cs - Fully updated for Emotion system, cleaned and organized
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;
using Momentum;

public class CompanionController : MonoBehaviour, IPuzzleInteractor
{
    [Header("Flight & Perception")]
    public float followDistance = 2.5f;
    public CompanionFlightController flightController { get; private set; }
    public CompanionPerception Perception { get; private set; }

    [Header("FSM & States")]
    public CompanionFSM fsm { get; private set; }
    public CompanionFollowState followState;
    public CompanionIdleState idleState;
    public CompanionMoveToPointState moveToPointState;

    [Header("Emotion & Visuals")]
    [SerializeField] private Light2D robotGlowLight;
    [SerializeField] private float chargedGlowIntensity = 1.2f;
    [SerializeField] private float glowFadeDuration = 1f;
    [SerializeField] private CompanionStatusUI statusUI;

    [Header("Cooldown & Lock")]
    [SerializeField] private float retargetCooldown = 0.2f;
    private float retargetCooldownTimer = 0f;
    private Coroutine emotionGlowCoroutine;
    private bool interactionLocked = false;

    [Header("Emotion State")]
    [SerializeField] private EmotionTag currentEmotion = EmotionTag.Neutral;

    // Interaction targets
    private IWorldInteractable currentTarget;
    private IWorldInteractable playerCommandTarget;

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

    public GameObject GetInteractorObject()
    {
        return gameObject;
    }

    public string GetDisplayName()
    {
        return "Companion";
    }


    #region Unity Lifecycle
    private void Awake()
    {
        InitializeComponents();
    }

    private void Start()
    {
        InputManager.instance.ToggleFollowPressed += ToggleFollowMode;
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
        flightController = GetComponent<CompanionFlightController>();
        Perception = GetComponent<CompanionPerception>();
        followState = new CompanionFollowState(this, fsm);
        idleState = new CompanionIdleState(this, fsm);
        moveToPointState = new CompanionMoveToPointState(this, fsm);
        fsm.Initialize(idleState, statusUI);
    }
    #endregion

    #region Player Commands
    public void CommandMoveToPoint(Vector2 worldPosition)
    {
        SetCurrentTarget(new TargetData(worldPosition));
        fsm.ChangeState(moveToPointState);
    }

    public void ToggleFollowMode()
    {
        if (fsm.CurrentStateType == CompanionStateType.Follow)
            fsm.ChangeState(idleState);
        else
            fsm.ChangeState(followState);
    }

    public void IssuePlayerCommand(IWorldInteractable target)
    {
        playerCommandTarget = target;
        SetCurrentTarget(new TargetData(target.GetTransform().position, target));
        fsm.ChangeState(new CompanionInvestigateState(this, fsm, target));
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

    #region Emotion Visual Feedback
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
