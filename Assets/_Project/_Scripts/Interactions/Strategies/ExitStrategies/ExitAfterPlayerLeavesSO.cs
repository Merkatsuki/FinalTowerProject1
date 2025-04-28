using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Exit/After Player Leaves Radius")]
public class ExitAfterPlayerLeavesSO : ExitStrategySO
{
    [SerializeField] private float radius = 5f;

    public override bool ShouldExit(IPuzzleInteractor actor, IWorldInteractable target)
    {
        GameObject player = GameObject.FindWithTag("Player");
        return Vector2.Distance(player.transform.position, target.GetTransform().position) > radius;
    }
}
