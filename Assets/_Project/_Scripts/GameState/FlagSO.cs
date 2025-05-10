using UnityEngine;

[CreateAssetMenu(menuName = "Game/Flag", fileName = "NewFlag")]
public class FlagSO : ScriptableObject
{
    public enum FlagType { Bool, Int, Float, String }


    public string displayName = "";
    public string description = "";
    public FlagType flagType = FlagType.Bool;
}
