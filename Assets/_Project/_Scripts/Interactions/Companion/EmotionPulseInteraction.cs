using UnityEngine;

[CreateAssetMenu(menuName = "RobotInteraction/Emotion Pulse")]
public class EmotionPulseInteraction : RobotInteractionSO
{
    public Color pulseColor = Color.cyan;

    public override void Execute(CompanionController companion, InteractableBase target)
    {
        Debug.Log($"[Robot Emotion Pulse]: {target.name} emits color {pulseColor}");
        // TODO: change companion's color, animate, or trigger emotion systems
    }
}
