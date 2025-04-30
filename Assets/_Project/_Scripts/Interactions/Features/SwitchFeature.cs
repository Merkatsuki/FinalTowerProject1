using UnityEngine;
using System.Collections.Generic;

public enum SwitchTriggerType
{
    OnInteract,
    OnTriggerEnter,
    OnTriggerStay,
    OnProximity
}

[RequireComponent(typeof(Collider2D))]
public class SwitchFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Trigger Type")]
    [SerializeField] private SwitchTriggerType triggerType = SwitchTriggerType.OnInteract;

    [Header("Activation Rules")]
    [SerializeField] private bool toggleable = true;
    [SerializeField] private bool startOn = false;
    [SerializeField] private bool oneTimeUse = false;
    [SerializeField] private bool staysOnAfterTrigger = true;
    [SerializeField] private List<string> acceptedTags = new() { "Player", "Companion" };

    [Header("Feature Effects")]
    [SerializeField] private List<EffectStrategySO> effectStrategies;

    [Header("Visual Feedback")]
    [SerializeField] private Animator switchAnimator;
    [SerializeField] private string toggleParameter = "On";

    private bool isActivated;
    private bool permanentlyUsed;
    private Transform player;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && (triggerType == SwitchTriggerType.OnTriggerEnter || triggerType == SwitchTriggerType.OnTriggerStay))
        {
            col.isTrigger = true;
        }

        isActivated = startOn;
        player = GameObject.FindWithTag("Player")?.transform;
        UpdateVisual();
    }

    private void Update()
    {
        if (triggerType == SwitchTriggerType.OnProximity && !permanentlyUsed && player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance <= 1.5f)
            {
                ActivateSwitch();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerType != SwitchTriggerType.OnTriggerEnter) return;
        if (acceptedTags.Contains(other.tag))
        {
            ActivateSwitch();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (triggerType != SwitchTriggerType.OnTriggerStay) return;
        if (acceptedTags.Contains(other.tag))
        {
            ActivateSwitch();
        }
    }

    public void OnInteract(IPuzzleInteractor actor)
    {
        if (triggerType != SwitchTriggerType.OnInteract) return;

        ActivateSwitch();
    }

    private void ActivateSwitch()
    {
        if (permanentlyUsed) return;
        if (!toggleable && isActivated) return;

        isActivated = toggleable ? !isActivated : true;

        RunEffects();

        if (oneTimeUse)
            permanentlyUsed = true;

        UpdateVisual();
    }

    private void RunEffects()
    {
        foreach (var effect in effectStrategies)
        {
            if (effect != null && TryGetComponent(out IWorldInteractable interactable))
            {
                effect.ApplyEffect(null, interactable, InteractionResult.Success);
            }
        }
    }

    private void UpdateVisual()
    {
        if (switchAnimator != null && !string.IsNullOrEmpty(toggleParameter))
        {
            switchAnimator.SetBool(toggleParameter, isActivated);
        }
    }

    public void SetEffectStrategies(List<EffectStrategySO> effects)
    {
        effectStrategies = effects ?? new();
    }

    public bool IsActivated() => isActivated;
    public bool IsPermanentlyUsed() => permanentlyUsed;
}
