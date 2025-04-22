using UnityEngine;

public class EnergyStateComponent : MonoBehaviour
{
    [SerializeField] private EnergyType currentEnergy = EnergyType.None;

    public EnergyType GetEnergy() => currentEnergy;

    public void SetEnergy(EnergyType energy)
    {
        currentEnergy = energy;
    }
}