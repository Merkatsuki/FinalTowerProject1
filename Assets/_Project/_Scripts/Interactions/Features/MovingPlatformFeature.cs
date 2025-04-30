using UnityEngine;
using DG.Tweening;
using Momentum;

public enum MovingPlatformActivationMode
{
    AlwaysOn,
    OnInteract,
    ToggleRunState,
    OnProximity,
    OnFlag
}

public enum PlatformState
{
    Idle,
    MovingForward,
    MovingBackward
}

public class MovingPlatformFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Path Settings")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float moveDuration = 3f;
    [SerializeField] private float waitTimeAtEnd = 1f;
    [SerializeField] private bool loop = true;

    [Header("Activation")]
    [SerializeField] private MovingPlatformActivationMode activationMode = MovingPlatformActivationMode.AlwaysOn;
    [SerializeField] private FlagSO requiredFlag;
    [SerializeField] private float activationProximity = 3f;

    [Header("Toggle Run State Settings")]
    [SerializeField] private bool isToggleActive = false; // Used only for ToggleRunState mode

    private Transform playerTransform;
    private Tween moveTween;
    private int currentTargetIndex = 0;
    private PlatformState platformState = PlatformState.Idle;
    private bool unlockedByFlag = false;

    private void Start()
    {
        playerTransform = FindFirstObjectByType<Player>()?.transform;

        if (activationMode == MovingPlatformActivationMode.AlwaysOn)
        {
            ActivateContinuous();
        }
        else if (activationMode == MovingPlatformActivationMode.OnFlag && requiredFlag != null)
        {
            if (PuzzleManager.Instance != null && PuzzleManager.Instance.IsFlagSet(requiredFlag))
            {
                unlockedByFlag = true;
                ActivateContinuous();
            }
        }
    }

    private void Update()
    {
        if (platformState != PlatformState.Idle || moveTween != null) return;

        if (activationMode == MovingPlatformActivationMode.OnProximity && playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (distance <= activationProximity)
            {
                MoveToNextWaypoint();
            }
        }

        if (activationMode == MovingPlatformActivationMode.ToggleRunState && isToggleActive)
        {
            MoveToNextWaypoint();
        }
    }

    public void OnInteract(IPuzzleInteractor actor)
    {
        if (activationMode == MovingPlatformActivationMode.OnInteract)
        {
            MoveToNextWaypoint();
        }
        else if (activationMode == MovingPlatformActivationMode.ToggleRunState)
        {
            isToggleActive = !isToggleActive;
        }
    }

    public void TriggerFromExternalSource()
    {
        if (activationMode == MovingPlatformActivationMode.OnInteract)
        {
            MoveToNextWaypoint();
        }
        else if (activationMode == MovingPlatformActivationMode.ToggleRunState)
        {
            isToggleActive = !isToggleActive;
        }
    }

    private void ActivateContinuous()
    {
        isToggleActive = true;
        MoveToNextWaypoint();
    }

    private void MoveToNextWaypoint()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        platformState = PlatformState.MovingForward;

        int nextIndex = (currentTargetIndex + 1) % waypoints.Length;
        Vector3 nextPosition = waypoints[nextIndex].position;

        moveTween = transform.DOMove(nextPosition, moveDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                currentTargetIndex = nextIndex;
                platformState = PlatformState.Idle;

                if ((activationMode == MovingPlatformActivationMode.AlwaysOn || isToggleActive) && (loop || currentTargetIndex != 0))
                {
                    Invoke(nameof(MoveToNextWaypoint), waitTimeAtEnd);
                }
            });
    }

    public bool IsMoving() => platformState != PlatformState.Idle;
    public int GetCurrentIndex() => currentTargetIndex;

    // Optional API for external triggers
    public void MoveForward()
    {
        if (platformState == PlatformState.Idle)
        {
            MoveToNextWaypoint();
        }
    }

    public void SetToggleActive(bool value)
    {
        isToggleActive = value;
    }

    public void UnlockPlatform()
    {
        if (activationMode == MovingPlatformActivationMode.OnFlag)
        {
            unlockedByFlag = true;
            ActivateContinuous();
        }
    }
}
