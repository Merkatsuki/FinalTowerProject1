using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }

    private HashSet<string> solvedPuzzles = new HashSet<string>();
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

    public void MarkPuzzleSolved(string key)
    {
        solvedPuzzles.Add(key);
        Debug.Log($"Puzzle '{key}' marked as solved.");
    }

    public bool IsPuzzleSolved(string key)
    {
        return solvedPuzzles.Contains(key);
    }

    //Plugin Hooks for Future Features
    public void ResetAllPuzzles()
    {
        solvedPuzzles.Clear();
    }

    public IEnumerable<string> GetAllSolvedPuzzles()
    {
        return solvedPuzzles;
    }

    public bool TrySolvePuzzle(string key, System.Func<bool> condition)
    {
        if (!solvedPuzzles.Contains(key) && condition())
        {
            MarkPuzzleSolved(key);
            return true;
        }
        return false;
    }

    public void SetFlag(string flagKey)
    {
        activeFlags.Add(flagKey);
    }

    public void ClearFlag(string flagKey)
    {
        activeFlags.Remove(flagKey);
    }

    public bool IsFlagSet(string flagKey)
    {
        return activeFlags.Contains(flagKey);
    }
}
