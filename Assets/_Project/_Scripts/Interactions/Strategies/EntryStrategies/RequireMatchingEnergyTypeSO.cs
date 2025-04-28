using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Entry/Require Matching Energy")]
public class RequireMatchingEnergyTypeSO : EntryStrategySO
{
    [SerializeField] private EnergyType requiredType;

    public override bool CanEnter(IPuzzleInteractor actor, IWorldInteractable target)
    {
        return actor.GetEnergyType() == requiredType;
    }
}