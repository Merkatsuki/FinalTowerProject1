using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Exit/After Timer")]
public class ExitAfterTimerSO : ExitStrategySO
{
    [SerializeField] private float waitTime = 3f;
    private float timer;

    public override void OnEnter(IPuzzleInteractor actor, IWorldInteractable target)
    {
        timer = 0f;
    }

    public override bool ShouldExit(IPuzzleInteractor actor, IWorldInteractable target)
    {
        timer += Time.deltaTime;
        return timer >= waitTime;
    }
}