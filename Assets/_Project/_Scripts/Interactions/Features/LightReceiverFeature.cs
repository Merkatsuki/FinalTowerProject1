using UnityEngine;

public class LightReceiverFeature : MonoBehaviour
{
    [SerializeField] private GameObject lightVisual;
    [SerializeField] private InteractableBase interactableTarget;
    [SerializeField] private bool isFinalReceiver = false;

    private bool isPowered = false; // current frame
    private bool lastPowered = false; // last frame

    // Logic pass only — no visuals or effects
    public void SetPowerLogicState(bool powered)
    {
        isPowered = powered;
    }

    // Apply final visual/effect logic based on delta
    public void FinalizePowerState()
    {
        if (isPowered != lastPowered)
        {
            Debug.Log($"[LightReceiverFeature] ({name}) Power state changed: {lastPowered} ➜ {isPowered}");
        }

        // Visual update
        lightVisual?.SetActive(isPowered);

        // Trigger effect only if state changed
        if (isPowered != lastPowered && interactableTarget != null)
        {
            Debug.Log($"[LightReceiverFeature] ({name}) Triggering interactable target: {interactableTarget.name}");
            interactableTarget.OnInteract(ReferenceManager.Instance.Player);
        }

        lastPowered = isPowered;
    }

    public bool IsPowered() => isPowered;
}
