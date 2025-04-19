using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }

    private HashSet<string> solvedPuzzles = new HashSet<string>();
    private readonly HashSet<string> activeFlags = new();

    private readonly Dictionary<string, PuzzleProfileSO> registeredProfiles = new();
    private readonly List<PuzzleProfileSO> activeProfiles = new();

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

    #region Solved State
    public void MarkPuzzleSolved(string key)
    {
        solvedPuzzles.Add(key);
        Debug.Log($"Puzzle '{key}' marked as solved.");
    }

    public bool IsPuzzleSolved(string key) => solvedPuzzles.Contains(key);

    public IEnumerable<string> GetAllSolvedPuzzles() => solvedPuzzles;

    public void ResetAllPuzzles()
    {
        solvedPuzzles.Clear();
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
    #endregion

    #region Flags
    public void SetFlag(string flagKey) => activeFlags.Add(flagKey);
    public void ClearFlag(string flagKey) => activeFlags.Remove(flagKey);
    public bool IsFlagSet(string flagKey) => activeFlags.Contains(flagKey);
    #endregion

    #region Puzzle Profile Management
    public void RegisterPuzzle(PuzzleProfileSO profile)
    {
        if (profile == null || registeredProfiles.ContainsKey(profile.puzzleId)) return;
        registeredProfiles[profile.puzzleId] = profile;
    }

    public void RegisterActivePuzzle(PuzzleProfileSO profile)
    {
        if (!activeProfiles.Contains(profile))
            activeProfiles.Add(profile);
    }

    public void UnregisterActivePuzzle(PuzzleProfileSO profile)
    {
        if (activeProfiles.Contains(profile))
            activeProfiles.Remove(profile);
    }

    public bool IsPuzzleAvailable(string puzzleId)
    {
        return registeredProfiles.ContainsKey(puzzleId) && !IsPuzzleSolved(puzzleId);
    }

    public PuzzleProfileSO GetPuzzleProfile(string puzzleId)
    {
        registeredProfiles.TryGetValue(puzzleId, out var profile);
        return profile;
    }

    public List<PuzzleProfileSO> GetActivePuzzles() => activeProfiles;
    #endregion
}