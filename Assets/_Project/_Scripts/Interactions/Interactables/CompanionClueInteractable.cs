using System.Collections.Generic;
using UnityEngine;

public class CompanionClueInteractable : InteractableBase, IPerceivable
{
    [SerializeField] private float priority = 5f;
    [SerializeField] private bool available = true;

    [Header("Companion Smart Interactions")]
    public List<RobotInteractionSO> robotInteractions;

    public float GetPriority() => priority;
    public bool IsAvailable() => available;
    public Transform GetTransform() => transform;

    public void RobotInteract(CompanionController companion)
    {
        foreach (var interaction in robotInteractions)
        {
            interaction.Execute(companion, this);
        }
    }
}
