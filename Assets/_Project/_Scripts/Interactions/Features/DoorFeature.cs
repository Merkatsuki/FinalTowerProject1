using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

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

    [Header("Linked Doors")]
    [SerializeField] private List<DoorFeature> linkedDoors = new();

    [Header("DoTween Door Movement")]
    [SerializeField] private Transform doorTransform;
    [SerializeField] private Vector3 openPosition;
    [SerializeField] private Vector3 closedPosition;
    [SerializeField] private float moveDuration = 0.4f;
    [SerializeField] private Ease moveEase = Ease.OutCubic;

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

    public void OpenDoorExternally()
    {
        if (!isOpen)
        {
            OpenDoor();
        }
    }
    private void OpenDoor()
    {
        operating = true;
        isOpen = true;

        if (doorTransform != null)
        {
            doorTransform.DOKill(); // Cancel any running tween
            doorTransform.DOMove(openPosition, moveDuration)
                .SetEase(moveEase)
                .OnComplete(() => operating = false);
        }

        RunFeatureEffects();

        if (autoClose)
            StartCoroutine(AutoCloseCoroutine());
        else
            operating = false;
    }

    private void CloseDoor()
    {
        operating = true;
        isOpen = false;

        if (doorTransform != null)
        {
            doorTransform.DOKill();
            doorTransform.DOMove(closedPosition, moveDuration)
                .SetEase(moveEase)
                .OnComplete(() => operating = false);
        }
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
