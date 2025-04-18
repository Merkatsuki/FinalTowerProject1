// ExitStrategySO.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Exit Strategy/After Timer")]
public class ExitAfterTimerSO : ExitStrategySO
{
    public float waitTime = 2f;
    private float timer;

    public override void OnEnter(CompanionController companion, CompanionClueInteractable target)
    {
        timer = waitTime;
    }

    public override bool ShouldExit(CompanionController companion, CompanionClueInteractable target)
    {
        timer -= Time.deltaTime;
        return timer <= 0;
    }
}

