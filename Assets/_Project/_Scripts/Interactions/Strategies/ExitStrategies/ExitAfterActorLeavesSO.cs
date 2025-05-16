using Momentum;
using UnityEngine;

public enum ExitSubject
{
    Player,
    Companion,
    Either
}

[CreateAssetMenu(menuName = "Strategies/Exit/Exit After Actor Leaves Area")]
public class ExitAfterActorLeavesSO : ExitStrategySO
{
    [SerializeField] private float radius = 3f;
    [SerializeField] private ExitSubject subject = ExitSubject.Player;
    [SerializeField] private bool inverse = false;

    public override bool ShouldExit(IPuzzleInteractor actor, IWorldInteractable target)
    {
        if (!(target is Component targetComponent)) return false;
        Transform targetTransform = targetComponent.transform;

        Transform playerTransform = null;
        Transform companionTransform = null;

        if (actor is Player player)
            playerTransform = player.transform;
        else if (actor is CompanionController companion)
            companionTransform = companion.transform;

        float playerDist = playerTransform != null
            ? Vector3.Distance(playerTransform.position, targetTransform.position)
            : float.MaxValue;

        float companionDist = companionTransform != null
            ? Vector3.Distance(companionTransform.position, targetTransform.position)
            : float.MaxValue;

        bool shouldExit = subject switch
        {
            ExitSubject.Player => playerDist > radius,
            ExitSubject.Companion => companionDist > radius,
            ExitSubject.Either => playerDist > radius && companionDist > radius,
            _ => false
        };

        return inverse ? !shouldExit : shouldExit;
    }
}
