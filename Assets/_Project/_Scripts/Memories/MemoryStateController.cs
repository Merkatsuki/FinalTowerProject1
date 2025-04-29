using UnityEngine;
using System;

public enum MemoryState
{
    Past,
    Present,
    Future
}

public class MemoryStateController : MonoBehaviour
{
    public static MemoryStateController Instance { get; private set; }

    public MemoryState CurrentMemoryState { get; private set; } = MemoryState.Present;

    public event Action<MemoryState> OnMemoryStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional: if you want memory state persistent between scenes
    }

    public void SetMemoryState(MemoryState newState)
    {
        if (CurrentMemoryState == newState) return;

        CurrentMemoryState = newState;
        Debug.Log($"[MemoryStateController] Memory state changed to: {newState}");

        OnMemoryStateChanged?.Invoke(CurrentMemoryState);
    }

    public void CycleMemoryState()
    {
        CurrentMemoryState = (MemoryState)(((int)CurrentMemoryState + 1) % Enum.GetValues(typeof(MemoryState)).Length);
        Debug.Log($"[MemoryStateController] Memory state cycled to: {CurrentMemoryState}");

        OnMemoryStateChanged?.Invoke(CurrentMemoryState);
    }
}
