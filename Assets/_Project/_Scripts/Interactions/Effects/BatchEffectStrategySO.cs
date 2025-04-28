using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BatchEffectStrategySO : EffectStrategySO
{
    [SerializeField] private List<EffectStrategySO> effects = new();

    public void SetEffects(List<EffectStrategySO> effectsList) => effects = effectsList;

    protected override async void ApplyEffectInternal(IPuzzleInteractor actor, IWorldInteractable interactable, InteractionResult result)
    {
        foreach (var effect in effects)
        {
            if (effect != null)
            {
                effect.ApplyEffect(actor, interactable, result);
                await Task.Yield(); // Small yield to allow next frame processing if needed
            }
        }
    }
}
