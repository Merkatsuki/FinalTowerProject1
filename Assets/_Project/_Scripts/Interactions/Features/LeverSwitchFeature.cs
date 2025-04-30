using UnityEngine;

public class LeverSwitchFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Switch Settings")]
    [SerializeField] private bool toggle = true;
    [SerializeField] private bool startOn = false;
    [SerializeField] private GameObject targetObject;
    [SerializeField] private Animator leverAnimator;

    private bool isOn;

    private void Start()
    {
        isOn = startOn;
        UpdateLeverVisual();
    }

    public void OnInteract(IPuzzleInteractor actor)
    {
        if (!toggle && isOn) return;

        isOn = !isOn;

        TriggerTarget();
        UpdateLeverVisual();
    }

    private void TriggerTarget()
    {
        if (targetObject == null) return;

        // ✅ Directly support our refactored MovingPlatformFeature
        if (targetObject.TryGetComponent(out MovingPlatformFeature platform))
        {
            platform.TriggerFromExternalSource();
            return;
        }

        // 🔄 Future extensibility: check for other target types here
        // e.g. door.Toggle(), light.Toggle(), etc.

        Debug.LogWarning("[LeverSwitchFeature] Target does not support expected behavior.");
    }

    private void UpdateLeverVisual()
    {
        if (leverAnimator == null) return;

        leverAnimator.SetBool("On", isOn);
    }
}
