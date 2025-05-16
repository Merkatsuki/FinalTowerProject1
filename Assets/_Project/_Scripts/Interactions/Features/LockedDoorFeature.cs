using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LockedDoorFeature : FeatureBase
{
    [Header("Door Settings")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private string openTriggerName = "Open";
    [SerializeField] private string closeTriggerName = "Close";
    [SerializeField] private string lockedTriggerName = "Locked"; // Optional locked visual

    [Header("Behavior Options")]
    [SerializeField] private bool toggleDoorOnInteract = false;
    [SerializeField] private bool autoClose = false;
    [SerializeField] private float autoCloseDelay = 2f;
    [SerializeField] private bool startLocked = true;

    [Header("Puzzle Requirement")]
    [SerializeField] private FlagSO requiredFlag; 

    private bool isOpen = false;
    private bool unlocked = false;
    private bool operating = false;

    private void Start()
    {
        // Initialize starting lock state
        unlocked = !startLocked;

        if (startLocked && doorAnimator != null && !string.IsNullOrEmpty(lockedTriggerName))
        {
            doorAnimator.SetTrigger(lockedTriggerName);
        }
    }

    public override void OnInteract(IPuzzleInteractor actor)
    {
        if (operating) return;

        if (!unlocked)
        {
            if (requiredFlag != null && FlagManager.Instance.IsFlagSet(requiredFlag))
            {
                UnlockDoor();
            }
            else
            {
                PlayLockedFeedback();
                return;
            }
        }

        if (toggleDoorOnInteract)
        {
            if (isOpen)
            {
                CloseDoor();
            }
            else
            {
                OpenDoor();
            }
        }
        else
        {
            OpenDoor();
        }
    }

    private void UnlockDoor()
    {
        unlocked = true;
        Debug.Log("[LockedDoorFeature] Door unlocked!");

        if (doorAnimator != null && !string.IsNullOrEmpty(openTriggerName))
        {
            doorAnimator.SetTrigger(openTriggerName);
        }

        RunFeatureEffects();
    }

    private void PlayLockedFeedback()
    {
        if (doorAnimator != null && !string.IsNullOrEmpty(lockedTriggerName))
        {
            doorAnimator.SetTrigger(lockedTriggerName);
        }
        else
        {
            Debug.Log("[LockedDoorFeature] Door is locked.");
        }
    }

    private void OpenDoor()
    {
        operating = true;

        if (doorAnimator != null && !string.IsNullOrEmpty(openTriggerName))
        {
            doorAnimator.SetTrigger(openTriggerName);
        }
        else
        {
            Debug.LogWarning("[LockedDoorFeature] No Animator or Open Trigger assigned!");
        }

        isOpen = true;

        RunFeatureEffects();

        if (autoClose)
        {
            StartCoroutine(AutoCloseCoroutine());
        }
        else
        {
            operating = false;
        }
    }

    private void CloseDoor()
    {
        operating = true;

        if (doorAnimator != null && !string.IsNullOrEmpty(closeTriggerName))
        {
            doorAnimator.SetTrigger(closeTriggerName);
        }
        else
        {
            Debug.LogWarning("[LockedDoorFeature] No Animator or Close Trigger assigned!");
        }

        isOpen = false;
        operating = false;
    }

    private IEnumerator AutoCloseCoroutine()
    {
        yield return new WaitForSeconds(autoCloseDelay);
        CloseDoor();
    }

    private void RunFeatureEffects()
    {
        foreach (var effect in featureEffects)
        {
            if (effect != null && TryGetComponent(out IWorldInteractable interactable))
            {
                effect.ApplyEffect(null, interactable, InteractionResult.Success);
            }
        }
    }

    public void SetDoorAnimator(Animator animator)
    {
        doorAnimator = animator;
    }
}
