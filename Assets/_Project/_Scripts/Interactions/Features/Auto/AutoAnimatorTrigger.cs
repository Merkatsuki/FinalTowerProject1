// AutoAnimatorTrigger.cs
using UnityEngine;

public class AutoAnimatorTrigger : AutoTriggerFeatureBase
{
    [SerializeField] private Animator animator;
    [SerializeField] private string triggerName;

    protected override void ExecuteTrigger()
    {
        if (animator != null && !string.IsNullOrEmpty(triggerName))
        {
            animator.SetTrigger(triggerName);
        }
        RunFeatureEffects();
    }
}
