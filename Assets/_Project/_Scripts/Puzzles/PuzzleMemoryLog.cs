using System.Collections.Generic;
using UnityEngine;

public class PuzzleMemoryLog : MonoBehaviour
{
    public static PuzzleMemoryLog Instance { get; private set; }

    private Dictionary<string, float> recentVisits = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void LogVisit(string clueId)
    {
        recentVisits[clueId] = Time.time;
    }

    public bool WasClueRecentlyVisited(string clueId, float cooldownTime)
    {
        if (recentVisits.TryGetValue(clueId, out float lastTime))
        {
            return Time.time - lastTime < cooldownTime;
        }
        return false;
    }

    public void ClearVisit(string clueId)
    {
        recentVisits.Remove(clueId);
    }
}
