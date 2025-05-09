// AutoSFXFeature.cs
using UnityEngine;

public class AutoSFXFeature : AutoTriggerFeature
{
    [SerializeField] private AudioClip sound;
    [SerializeField] private AudioSource audioSource;

    protected override void ExecuteTrigger()
    {
        if (audioSource != null && sound != null)
        {
            audioSource.PlayOneShot(sound);
        }
        RunFeatureEffects();
    }
}
