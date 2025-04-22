
using UnityEngine;

public interface IPuzzleInteractor
{
    GameObject GetInteractorObject();
    EnergyType GetEnergyType();
    string GetDisplayName(); 
}