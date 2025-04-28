using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Play Sound Effect")]
public class PlaySoundEffect : EffectStrategySO
{
    [SerializeField] private AudioClip sound;
    [SerializeField] private float volume = 1.0f;

    protected override void ApplyEffectInternal(IPuzzleInteractor actor, IWorldInteractable interactable, InteractionResult result)
    {
        if (sound != null && interactable != null)
        {
            AudioSource.PlayClipAtPoint(sound, interactable.GetTransform().position, volume);
            Debug.Log($"[Effect] Played sound: {sound.name}");
        }
    }
}