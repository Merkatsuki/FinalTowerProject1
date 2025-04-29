using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Delay Effect")]
public class DelayEffect : EffectStrategySO
{
    [SerializeField] private float delaySeconds = 1f;

    protected override async void ApplyEffectInternal(IPuzzleInteractor actor, IWorldInteractable interactable, InteractionResult result)
    {
        await Task.Delay((int)(delaySeconds * 1000f));
        Debug.Log($"[Effect] Delay of {delaySeconds} seconds completed.");
    }

    public void SetDelay(float seconds)
    {
        delaySeconds = seconds;
    }
}
