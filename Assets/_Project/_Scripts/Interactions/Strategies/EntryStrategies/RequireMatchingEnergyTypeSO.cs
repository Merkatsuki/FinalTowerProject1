using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Entry/Require Matching Energy")]
public class RequireMatchingEnergyTypeSO : EntryStrategySO
{
    [SerializeField] private EnergyType requiredEnergyType;

    public override bool CanEnter(IPuzzleInteractor actor, IWorldInteractable target)
    {
        return actor.GetEnergyType() == requiredEnergyType;
    }

    public void SetRequiredEnergyType(EnergyType energy)
    {
        requiredEnergyType = energy;
    }
}