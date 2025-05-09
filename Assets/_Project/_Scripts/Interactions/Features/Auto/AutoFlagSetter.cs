// AutoFlagSetter.cs
using UnityEngine;

public class AutoFlagSetter : AutoTriggerFeature
{
    [SerializeField] private string flagKey;
    [SerializeField] private bool flagValue = true;

    protected override void ExecuteTrigger()
    {
        if (!string.IsNullOrEmpty(flagKey))
        {
            // Replace with your flag/blackboard system
            Debug.Log($"[Flag] Set '{flagKey}' = {flagValue}");
            // Example: GameFlags.Set(flagKey, flagValue);
        }
        RunFeatureEffects();
    }
}
