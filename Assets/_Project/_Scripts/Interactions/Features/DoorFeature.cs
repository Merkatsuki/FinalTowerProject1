using UnityEngine;
using System.Collections.Generic;

public class DoorFeature : MonoBehaviour, IInteractableFeature
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private float closeDelay = 2.0f;

    [Header("Feature Effects")]
    [SerializeField] private List<EffectStrategySO> featureEffects = new();

    private bool isOpen = false;

    private void Awake()
    {
        if (doorAnimator == null)
        {
            doorAnimator = GetComponent<Animator>();
            if (doorAnimator == null)
            {
                doorAnimator = gameObject.AddComponent<Animator>();
                Debug.LogWarning("[DoorFeature] No Animator found. Added default Animator component.");
            }
        }
    }

    public void OnInteract(IPuzzleInteractor actor)
    {
        OpenDoor(actor);
    }

    public void OpenDoor(IPuzzleInteractor actor)
    {
        if (isOpen) return;

        isOpen = true;
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Open");
            Invoke(nameof(CloseDoor), closeDelay);
            Debug.Log("[DoorFeature] Door opened.");
        }

        RunFeatureEffects(actor);
    }

    private void CloseDoor()
    {
        if (!isOpen) return;

        isOpen = false;
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Close");
            Debug.Log("[DoorFeature] Door closed.");
        }
    }

    private void RunFeatureEffects(IPuzzleInteractor actor)
    {
        foreach (var effect in featureEffects)
        {
            if (effect != null && TryGetComponent(out IWorldInteractable interactable))
            {
                effect.ApplyEffect(actor, interactable, InteractionResult.Success);
            }
        }
    }

    public void SetFeatureEffects(List<EffectStrategySO> effects)
    {
        featureEffects = effects ?? new List<EffectStrategySO>();
    }

    public bool IsOpen() => isOpen;
}
