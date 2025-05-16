using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Exit/After Timer Then Conditional")]
public class ExitAfterTimerThenConditionalSO : ExitStrategySO
{
    [SerializeField] private ExitStrategySO conditionStrategy;
    [SerializeField] private float minTime = 2f;
    private float timer;

    public override bool ShouldExit(IPuzzleInteractor actor, IWorldInteractable target)
    {
        timer += Time.deltaTime;

        if (timer < minTime)
            return false;

        if (conditionStrategy != null)
            return conditionStrategy.ShouldExit(actor, target);

        return true;
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