// Refactored PuzzleManager.cs -> FlagManager.cs
using UnityEngine;
using System.Collections.Generic;

public class FlagManager : MonoBehaviour
{
    public static FlagManager Instance { get; private set; }

    private readonly Dictionary<FlagSO, bool> boolFlags = new();
    private readonly Dictionary<FlagSO, int> intFlags = new();
    private readonly Dictionary<FlagSO, float> floatFlags = new();
    private readonly Dictionary<FlagSO, string> stringFlags = new();


    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void SetBool(FlagSO flag, bool value)
    {
        if (flag == null || flag.flagType != FlagSO.FlagType.Bool) return;
        boolFlags[flag] = value;
    }

    public void SetInt(FlagSO flag, int value)
    {
        if (flag == null || flag.flagType != FlagSO.FlagType.Int) return;
        intFlags[flag] = value;
    }

    public void SetFloat(FlagSO flag, float value)
    {
        if (flag == null || flag.flagType != FlagSO.FlagType.Float) return;
        floatFlags[flag] = value;
    }

    public void SetString(FlagSO flag, string value)
    {
        if (flag == null || flag.flagType != FlagSO.FlagType.String) return;
        stringFlags[flag] = value;
    }

    public bool GetBool(FlagSO flag)
    {
        if (flag == null || flag.flagType != FlagSO.FlagType.Bool) return false;
        return boolFlags.TryGetValue(flag, out bool value) && value;
    }

    public int GetInt(FlagSO flag)
    {
        if (flag == null || flag.flagType != FlagSO.FlagType.Int) return 0;
        return intFlags.TryGetValue(flag, out int value) ? value : 0;
    }

    public float GetFloat(FlagSO flag)
    {
        if (flag == null || flag.flagType != FlagSO.FlagType.Float) return 0f;
        return floatFlags.TryGetValue(flag, out float value) ? value : 0f;
    }

    public string GetString(FlagSO flag)
    {
        if (flag == null || flag.flagType != FlagSO.FlagType.String) return string.Empty;
        return stringFlags.TryGetValue(flag, out string value) ? value : string.Empty;
    }

    public bool IsFlagSet(FlagSO flag)
    {
        return flag != null && flag.flagType == FlagSO.FlagType.Bool && GetBool(flag);
    }
}
