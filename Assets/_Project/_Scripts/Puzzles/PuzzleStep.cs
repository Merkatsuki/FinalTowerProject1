using UnityEngine;

[System.Serializable]
public class PuzzleStep
{
    public string stepId;
    public PuzzleObject targetObject;
    public EnergyType requiredEnergy = EnergyType.None;
    public bool markStepWhenInteractionTriggered = true;
    public string flagToSet;
}