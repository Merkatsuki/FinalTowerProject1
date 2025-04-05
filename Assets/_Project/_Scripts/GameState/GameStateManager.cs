using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    private List<IGameStateListener> listeners = new List<IGameStateListener>();

    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterListener(IGameStateListener listener)
    {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    public void UnregisterListener(IGameStateListener listener)
    {
        listeners.Remove(listener);
    }

    public void SetState(GameState newState)
    {
        if (CurrentState == newState) return;
        CurrentState = newState;
        foreach (var listener in listeners)
        {
            listener.OnGameStateChanged(newState);
        }
    }

    public bool Is(GameState state) => CurrentState == state;
}
