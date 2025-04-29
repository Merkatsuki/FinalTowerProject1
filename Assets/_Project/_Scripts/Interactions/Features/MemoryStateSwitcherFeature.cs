using UnityEngine;

public class MemoryStateSwitcherFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Switch Settings")]
    [SerializeField] private bool cycleStates = true;
    [SerializeField] private MemoryState targetState = MemoryState.Past;

    public void OnInteract(IPuzzleInteractor actor)
    {
        if (MemoryStateController.Instance == null)
        {
            Debug.LogWarning("[MemoryStateSwitcherFeature] No MemoryStateController found!");
            return;
        }

        if (cycleStates)
        {
            MemoryStateController.Instance.CycleMemoryState();
        }
        else
        {
            MemoryStateController.Instance.SetMemoryState(targetState);
        }
    }
}

