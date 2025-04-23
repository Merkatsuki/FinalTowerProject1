using UnityEngine;

public class EnergyDockingZone : InteractableBase
{
    [SerializeField] private EnergyType zoneEnergyType = EnergyType.None;
    [SerializeField] private bool isOneTimeUse = false;
    private bool hasBeenUsed = false;

    public override void OnInteract(IPuzzleInteractor actor)
    {
        if (hasBeenUsed && isOneTimeUse) return;
        if (actor is CompanionController companion)
        {
            var energyComp = companion.GetComponent<EnergyStateComponent>();
            if (energyComp != null)
            {
                energyComp.SetEnergy(zoneEnergyType);
                Debug.Log($"[DockingZone] {companion.name} charged with {zoneEnergyType}");
                hasBeenUsed = true;
            }
        }
    }
}