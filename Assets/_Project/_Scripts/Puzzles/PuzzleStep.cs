using UnityEngine;

[System.Serializable]
public class PuzzleStep
{
    public string stepId;

    [Tooltip("Clue or interactable that this step is bound to.")]
    public CompanionClueInteractable targetClue;

    [Tooltip("Interaction required to complete this step.")]
    public RobotInteractionSO requiredInteraction;

    [Tooltip("Require a specific energy color on the robot.")]
    public EnergyType requiredEnergy = EnergyType.None;

    [Tooltip("Optional: Set this flag if step completes.")]
    public string flagToSet;

    [Tooltip("Whether to auto-complete on valid interaction trigger.")]
    public bool markStepWhenInteractionTriggered = true;

}