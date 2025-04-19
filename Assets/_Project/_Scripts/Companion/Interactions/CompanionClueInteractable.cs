using System.Collections.Generic;
using UnityEngine;

public class CompanionClueInteractable : MonoBehaviour, IRobotPerceivable, IHoverProfileProvider
{
    [SerializeField] private float priority = 5f;
    [SerializeField] private HoverStagingProfileSO hoverProfile;
    private HashSet<RobotInteractionSO> usedInteractions = new();


    [Header("Companion Smart Interactions")]
    public List<RobotInteractionSO> robotInteractions;

    private bool isHandled = false;

    public List<RobotInteractionSO> GetRobotInteractions() => robotInteractions;
    public HoverStagingProfileSO GetHoverProfile() => hoverProfile;
    public float GetPriority() => priority;
    public Transform GetTransform() => transform;

    public void MarkHandled() => isHandled = true;
    public void ResetHandled()
    {
        isHandled = false;
    }

    public void RobotInteract(CompanionController companion)
    {
        foreach (var interaction in robotInteractions)
        {
            interaction.Execute(companion, this); // 'this' is a CompanionClueInteractable
        }
    }
    public bool HasValidInteractions(CompanionController companion)
    {
        foreach (var interaction in robotInteractions)
        {
            if (!HasUsedInteraction(interaction) && interaction.CanExecute(companion, this))
                return true;
        }
        return false;
    }

    public void MarkInteractionUsed(RobotInteractionSO interaction)
    {
        if (interaction != null && !interaction.IsRepeatable)
            usedInteractions.Add(interaction);
    }

    public bool HasUsedInteraction(RobotInteractionSO interaction)
    {
        return usedInteractions.Contains(interaction);
    }
}

