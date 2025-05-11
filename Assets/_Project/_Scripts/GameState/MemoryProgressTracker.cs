using System.Collections.Generic;
using UnityEngine;

public class MemoryProgressTracker : MonoBehaviour
{
    public static MemoryProgressTracker Instance;

    public enum MemoryZoneID { Joy, Anger, Sadness, FinalMemory }

    [SerializeField] private List<MemoryZoneID> trackedZones = new() { MemoryZoneID.Joy, MemoryZoneID.Anger, MemoryZoneID.Sadness };
    private HashSet<MemoryZoneID> completedZones = new();

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

    public void MarkZoneComplete(MemoryZoneID zone)
    {
        if (!completedZones.Contains(zone))
        {
            completedZones.Add(zone);
            Debug.Log($"Zone marked complete: {zone}");
        }
    }

    public bool IsZoneComplete(MemoryZoneID zone) => completedZones.Contains(zone);

    public bool AreAllZonesComplete()
    {
        foreach (var zone in trackedZones)
        {
            if (!completedZones.Contains(zone))
                return false;
        }
        return true;
    }
}
