using UnityEngine;

[CreateAssetMenu(menuName = "Puzzle/Flag")]
public class FlagSO : ScriptableObject
{
    public string flagName;
    public bool isInteger;
    public int defaultIntValue;
    public bool defaultBoolValue;
}
