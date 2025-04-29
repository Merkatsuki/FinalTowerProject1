using UnityEngine;

public class MemoryStateAffectableFeature : MonoBehaviour
{
    [Header("State Visuals")]
    [SerializeField] private GameObject pastStateObject;
    [SerializeField] private GameObject presentStateObject;
    [SerializeField] private GameObject futureStateObject;

    private void Start()
    {
        if (MemoryStateController.Instance != null)
        {
            MemoryStateController.Instance.OnMemoryStateChanged += UpdateVisualState;
            UpdateVisualState(MemoryStateController.Instance.CurrentMemoryState);
        }
        else
        {
            Debug.LogWarning("[MemoryStateAffectableFeature] No MemoryStateController found at start!");
        }
    }

    private void OnDestroy()
    {
        if (MemoryStateController.Instance != null)
        {
            MemoryStateController.Instance.OnMemoryStateChanged -= UpdateVisualState;
        }
    }

    private void UpdateVisualState(MemoryState state)
    {
        if (pastStateObject != null)
            pastStateObject.SetActive(state == MemoryState.Past);

        if (presentStateObject != null)
            presentStateObject.SetActive(state == MemoryState.Present);

        if (futureStateObject != null)
            futureStateObject.SetActive(state == MemoryState.Future);
    }
}
