using UnityEngine;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }
    private readonly HashSet<string> activeFlags = new();

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


    public void SetFlag(string flag)
    {
        if (!string.IsNullOrEmpty(flag))
        {
            activeFlags.Add(flag);
        }
    }

    public bool IsFlagSet(string flag)
    {
        return activeFlags.Contains(flag);
    }

    public void ResetFlags()
    {
        activeFlags.Clear();
    }
}