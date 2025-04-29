using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }

    private readonly HashSet<string> boolFlags = new();
    private readonly Dictionary<string, int> intFlags = new();

    public List<string> GetAllBoolFlags()
    {
        return boolFlags.ToList();
    }

    public Dictionary<string, int> GetAllIntFlags()
    {
        return new Dictionary<string, int>(intFlags);
    }


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

    // --- String Compatibility Layer ---

    public void SetFlag(string flag)
    {
        if (!string.IsNullOrEmpty(flag))
        {
            boolFlags.Add(flag);
            Debug.Log($"[PuzzleManager] Legacy string flag '{flag}' set TRUE.");
        }
    }

    public bool IsFlagSet(string flag)
    {
        return boolFlags.Contains(flag);
    }

    // --- FlagSO API ---

    public void SetFlag(FlagSO flag, bool value)
    {
        if (flag == null) return;

        if (flag.isInteger)
        {
            intFlags[flag.flagName] = value ? 1 : 0;
            Debug.Log($"[PuzzleManager] Integer flag '{flag.flagName}' set to {(value ? 1 : 0)}.");
        }
        else
        {
            if (value)
                boolFlags.Add(flag.flagName);
            else
                boolFlags.Remove(flag.flagName);

            Debug.Log($"[PuzzleManager] Bool flag '{flag.flagName}' set to {value}.");
        }
    }

    public void SetFlagValue(FlagSO flag, int value)
    {
        if (flag == null || !flag.isInteger) return;
        intFlags[flag.flagName] = value;
        Debug.Log($"[PuzzleManager] Integer flag '{flag.flagName}' set to {value}.");
    }

    public bool IsFlagSet(FlagSO flag)
    {
        if (flag == null) return false;

        if (flag.isInteger)
            return intFlags.TryGetValue(flag.flagName, out int val) && val != 0;
        else
            return boolFlags.Contains(flag.flagName);
    }

    public int GetFlagValue(FlagSO flag)
    {
        if (flag == null || !flag.isInteger) return 0;
        intFlags.TryGetValue(flag.flagName, out var value);
        return value;
    }

    // --- Reset ---

    public void ResetFlags()
    {
        boolFlags.Clear();
        intFlags.Clear();
        Debug.Log("[PuzzleManager] All flags reset.");
    }
}
