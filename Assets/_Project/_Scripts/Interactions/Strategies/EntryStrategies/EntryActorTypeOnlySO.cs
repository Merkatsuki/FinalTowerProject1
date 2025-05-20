using UnityEngine;

[CreateAssetMenu(menuName = "EntryStrategies/Allow Only Actor Type")]
public class EntryActorTypeOnlySO : EntryStrategySO
{
    public enum ActorType { Player, Companion }

    [SerializeField] private ActorType allowedType;

    public override bool CanEnter(IPuzzleInteractor actor, IWorldInteractable target)
    {
        string actorName = actor.GetDisplayName().ToLower();
        switch (allowedType)
        {
            case ActorType.Player:
                return actorName.Contains("player");
            case ActorType.Companion:
                return actorName.Contains("companion");
            default:
                return false;
        }
    }
}
