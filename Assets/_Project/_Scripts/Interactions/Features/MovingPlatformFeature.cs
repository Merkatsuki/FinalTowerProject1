using UnityEngine;
using DG.Tweening;

public enum PlatformMovementMode
{
    Continuous,
    MoveToNext,
    MoveUntilToggled
}

public enum PlatformActivationMode
{
    AlwaysOn,
    OnInteract,
    OnActivated
}

public class MovingPlatformFeature : FeatureBase
{
    [Header("Platform Movement Settings")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float moveDuration = 2f;
    [SerializeField] private float waitTimeAtWaypoint = 0.5f;
    [SerializeField] private bool loopWaypoints = true;

    [Header("Platform Object (What Moves)")]
    [SerializeField] private Transform platformTransform;

    [Header("Mode Settings")]
    [SerializeField] private PlatformMovementMode movementMode = PlatformMovementMode.Continuous;
    [SerializeField] private PlatformActivationMode activationMode = PlatformActivationMode.AlwaysOn;
    [SerializeField] private bool isInitiallyLocked = false;

    private int currentWaypointIndex = 0;
    private bool isMoving = false;
    private bool isActive = false;
    private bool isLocked = false;
    private Tween moveTween;

    private void Start()
    {
        isLocked = isInitiallyLocked;

        if (activationMode == PlatformActivationMode.AlwaysOn && !isLocked)
        {
            ActivatePlatform();
        }
    }

    private void Update()
    {
        if (isMoving || isLocked || waypoints.Length < 2) return;

        switch (movementMode)
        {
            case PlatformMovementMode.Continuous:
            case PlatformMovementMode.MoveUntilToggled:
                if (isActive)
                    MoveToNext();
                break;
        }
    }

    public override void OnInteract(IPuzzleInteractor actor)
    {
        if (activationMode != PlatformActivationMode.OnInteract || isLocked) return;
        HandleActivation();
    }

    public void TriggerFromExternal(IPuzzleInteractor actor = null)
    {
        if (activationMode != PlatformActivationMode.OnActivated || isLocked) return;
        HandleActivation();
    }

    private void HandleActivation()
    {
        if (isMoving || waypoints.Length < 2) return;

        switch (movementMode)
        {
            case PlatformMovementMode.MoveToNext:
                MoveToNext();
                break;
            case PlatformMovementMode.MoveUntilToggled:
                isActive = !isActive;
                break;
        }
    }

    private void MoveToNext()
    {
        isMoving = true;

        int nextIndex = (currentWaypointIndex + 1) % waypoints.Length;
        Vector3 targetPosition = waypoints[nextIndex].position;

        moveTween = platformTransform.DOMove(targetPosition, moveDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                currentWaypointIndex = nextIndex;
                isMoving = false;

                if (!loopWaypoints && currentWaypointIndex == waypoints.Length - 1)
                {
                    isActive = false;
                }

                if (movementMode == PlatformMovementMode.MoveToNext)
                {
                    isActive = false;
                }
            });
    }

    public void UnlockPlatform()
    {
        isLocked = false;
        if (activationMode == PlatformActivationMode.AlwaysOn)
        {
            ActivatePlatform();
        }
    }

    private void ActivatePlatform()
    {
        isActive = true;
    }

    public void SetWaypoints(Transform[] points) => waypoints = points;
    public void SetPlatformTransform(Transform platform) => platformTransform = platform;

    // Public accessors
    public bool IsLocked() => isLocked;
    public bool IsMoving() => isMoving;
    public bool IsActive() => isActive;
    public int GetCurrentWaypointIndex() => currentWaypointIndex;
}
