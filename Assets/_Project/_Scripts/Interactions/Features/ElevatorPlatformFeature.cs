using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;


[System.Serializable]
public class GearLockAnchor
{
    public Transform stopTransform;
    public bool isUnlocked = false;
    public GearSpinFeature gearVisual;
}

public class ElevatorPlatformFeature : FeatureBase
{
    [Header("Elevator Settings")]
    [SerializeField] private float maxVelocity = 3f;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private Transform elevatorPlatform; 
    [SerializeField] private Transform minHeightAnchor;
    [SerializeField] private Transform maxHeightAnchor;

    [Header("Gear Lock Anchors")]
    [SerializeField] private List<GearLockAnchor> gearLocks = new();

    private bool playerInControlZone = false;
    private bool isElevatorActive = true;
    private bool isMoving = false;
    private bool allowControl = true;
    private float currentVelocityY = 0f;


    private void Update()
    {
        if (!isElevatorActive || !playerInControlZone || !allowControl) return;

        var input = Momentum.InputManager.instance;
        if (input != null)
        {
            float verticalInput = input.WASDInput.y;
            HandleElevatorMovement(verticalInput);
        }
    }

    private void HandleElevatorMovement(float inputDirection)
    {
        float currentY = elevatorPlatform.position.y;

        // Determine stopping conditions
        if (inputDirection > 0f)
        {
            foreach (var gear in gearLocks)
            {
                if (!gear.isUnlocked && currentY >= gear.stopTransform.position.y)
                {
                    SetGearSpinActive(false);
                    currentVelocityY = 0;
                    return;
                }
            }

            if (currentY >= maxHeightAnchor.position.y)
            {
                SetGearSpinActive(false);
                currentVelocityY = 0;
                return;
            }
        }
        else if (inputDirection < 0f && currentY <= minHeightAnchor.position.y)
        {
            SetGearSpinActive(false);
            currentVelocityY = 0;
            return;
        }

        // Accelerate or decelerate
        if (Mathf.Abs(inputDirection) > 0.1f)
        {
            currentVelocityY += inputDirection * acceleration * Time.deltaTime;
        }
        else
        {
            float decel = deceleration * Time.deltaTime;
            if (currentVelocityY > 0f) currentVelocityY = Mathf.Max(0, currentVelocityY - decel);
            else if (currentVelocityY < 0f) currentVelocityY = Mathf.Min(0, currentVelocityY + decel);
        }

        // Clamp velocity
        currentVelocityY = Mathf.Clamp(currentVelocityY, -maxVelocity, maxVelocity);

        // Move platform
        if (Mathf.Abs(currentVelocityY) > 0.01f)
        {
            isMoving = true;
            Vector3 newPos = elevatorPlatform.position + new Vector3(0, currentVelocityY * Time.deltaTime, 0);
            newPos.y = Mathf.Clamp(newPos.y, minHeightAnchor.position.y, maxHeightAnchor.position.y);
            elevatorPlatform.position = newPos;
            SetGearSpinActive(true);
        }
        else
        {
            isMoving = false;
            SetGearSpinActive(false);
        }
    }

    public void SetGearLockState(Transform anchor, bool unlocked)
    {
        foreach (var gear in gearLocks)
        {
            if (gear.stopTransform == anchor)
            {
                gear.isUnlocked = unlocked;
                Debug.Log($"[Elevator] Gear at {anchor.name} set to: {(unlocked ? "UNLOCKED" : "LOCKED")}");
                return;
            }
        }
    }

    private void SetGearSpinActive(bool spinning)
    {
        foreach (var gear in gearLocks)
        {
            if (gear.isUnlocked && gear.gearVisual != null)
            {
                gear.gearVisual.SetSpinning(spinning);

                if (spinning)
                {
                    // Use currentVelocityY as the basis for rotation speed
                    float rotationMultiplier = 15f; // Adjust this for visual tuning
                    gear.gearVisual.speed_rotation = -currentVelocityY * rotationMultiplier;

                    // Flip visual if needed based on direction
                    float newSpeed = gear.gearVisual.speed_rotation;
                    if (gear.gearVisual.flipYOnPositiveRotation)
                    {
                        var originalScale = gear.gearVisual.transform.localScale;
                        gear.gearVisual.transform.localScale = new Vector3(
                            originalScale.x,
                            newSpeed > 0 ? -Mathf.Abs(originalScale.y) : Mathf.Abs(originalScale.y),
                            originalScale.z
                        );
                    }
                }
            }
        }
    }

    public void SetAllowControl(bool allowed)
    {
        allowControl = allowed;
        Debug.Log($"[Elevator] Control allowed: {allowControl}");
    }

    public void UnlockGearAt(Transform targetAnchor)
    {
        foreach (var gear in gearLocks)
        {
            if (gear.stopTransform == targetAnchor)
            {
                gear.isUnlocked = true;
                Debug.Log($"[ElevatorPlatformFeature] Gear unlocked at {targetAnchor.name}");
                return;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SetPlayerInControlZone(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SetPlayerInControlZone(false);
        }
    }

    private void SetPlayerInControlZone(bool inZone)
    {
        playerInControlZone = inZone;
        Debug.Log($"[ElevatorPlatformFeature] Player in control zone: {inZone}");
    }

    public override void OnInteract(IPuzzleInteractor actor)
    {
        Debug.Log($"[ElevatorPlatformFeature] Elevator activated via interact by {actor}");
        NotifyPuzzleInteractionSuccess();
        RunFeatureEffects(actor);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (var gear in gearLocks)
        {
            if (gear.stopTransform != null)
            {
                Gizmos.DrawLine(
                    new Vector3(-100, gear.stopTransform.position.y, 0),
                    new Vector3(100, gear.stopTransform.position.y, 0)
                );
            }
        }

        Gizmos.color = Color.green;
        if (minHeightAnchor != null)
            Gizmos.DrawSphere(minHeightAnchor.position, 0.1f);
        if (maxHeightAnchor != null)
            Gizmos.DrawSphere(maxHeightAnchor.position, 0.1f);
    }
}
