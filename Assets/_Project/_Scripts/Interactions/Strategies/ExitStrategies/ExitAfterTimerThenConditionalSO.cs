using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Exit/After Timer Then Conditional")]
public class ExitAfterTimerThenConditionalSO : ExitStrategySO
{
    [SerializeField] private float minTime = 2f;
    private float timer;

    public override bool ShouldExit(IPuzzleInteractor actor, IWorldInteractable target)
    {
        timer += Time.deltaTime;
        return timer >= minTime;
    }

    public override void OnEnter(IPuzzleInteractor actor, IWorldInteractable target)
    {
        timer = 0f;
    }

    public void SetMinTime(float seconds)
    {
        minTime = seconds;
    }

}