using UnityEngine;

public class EnergyPuzzleGate : InteractableBase
{
    [SerializeField] private EnergyType requiredEnergy = EnergyType.None;
    [SerializeField] private GameObject gateBarrier;

    private bool isUnlocked = false;

    public override bool CanBeInteractedWith(IPuzzleInteractor actor)
    {
        if (isUnlocked) return false;
        if (actor.GetEnergyType() != requiredEnergy) return false;

        foreach (var strategy in entryStrategies)
        {
            if (strategy != null && !strategy.CanEnter(actor, this))
                return false;
        }

        return true;
    }

    public override void OnInteract(IPuzzleInteractor actor)
    {
        if (!CanBeInteractedWith(actor)) return;

        isUnlocked = true;
        if (gateBarrier != null)
        {
            gateBarrier.SetActive(false);
        }

        Debug.Log($"[EnergyPuzzleGate] Unlocked by {actor.GetDisplayName()} with {actor.GetEnergyType()}");
    }
}