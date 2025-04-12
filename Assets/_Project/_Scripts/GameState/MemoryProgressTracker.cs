using System.Collections.Generic;
using UnityEngine;

public class MemoryProgressTracker : MonoBehaviour
{
    public static MemoryProgressTracker Instance;

    [Header("Tracking")]
    [SerializeField] private List<string> trackedMemoryScenes = new List<string> { "Memory1", "Memory2", "Memory3" };
    [SerializeField] private string finalMemoryScene = "MemoryFinal";

    private HashSet<string> completedMemories = new HashSet<string>();

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

    public void MarkMemoryComplete(string sceneName)
    {
        if (!completedMemories.Contains(sceneName))
        {
            completedMemories.Add(sceneName);
            Debug.Log($"Memory marked complete: {sceneName}");
        }
    }

    public bool IsMemoryComplete(string sceneName) => completedMemories.Contains(sceneName);

    public bool AreAllTrackedMemoriesComplete()
    {
        foreach (var memory in trackedMemoryScenes)
        {
            if (!completedMemories.Contains(memory))
                return false;
        }

        return true;
    }
}
