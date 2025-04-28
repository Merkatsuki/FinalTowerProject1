using UnityEngine;

public interface ISharedEffectSettings
{
    void SetDelay(float seconds);
    void SetSoundCue(AudioClip clip);
}