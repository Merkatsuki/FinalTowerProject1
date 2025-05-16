using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorFeature : FeatureBase
{
    [Header("Door Settings")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private string openTriggerName = "Open";
    [SerializeField] private string closeTriggerName = "Close";

    [Header("Behavior Options")]
    [SerializeField] private bool toggleDoorOnInteract = false;
    [SerializeField] private bool autoClose = false;
    [SerializeField] private float autoCloseDelay = 2f;
    [SerializeField] private bool startClosed = true;

    private bool isOpen = false;
    private bool operating = false;

    private void Start()
    {
        // Optional: force closed animation on start
        if (startClosed && doorAnimator != null && !string.IsNullOrEmpty(closeTriggerName))
        {
            doorAnimator.SetTrigger(closeTriggerName);
            isOpen = false;
        }
    }

    public override void OnInteract(IPuzzleInteractor actor)
    {
        if (operating) return;

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

    private void OpenDoor()
    {
        operating = true;

        if (doorAnimator != null && !string.IsNullOrEmpty(openTriggerName))
        {
            doorAnimator.SetTrigger(openTriggerName);
        }
        else
        {
            Debug.LogWarning("[DoorFeature] No Animator or Open Trigger assigned!");
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
            Debug.LogWarning("[DoorFeature] No Animator or Close Trigger assigned!");
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
