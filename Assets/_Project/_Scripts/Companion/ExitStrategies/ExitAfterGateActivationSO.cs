using UnityEngine;

[CreateAssetMenu(menuName = "Exit Strategy/Exit After Gate Activation")]
public class ExitAfterGateActivationSO : ExitStrategySO
{
    private EnergyPuzzleGate gate;
    private bool activated;

    public override void OnEnter(CompanionController companion, CompanionClueInteractable target)
    {
        activated = false;

        if (target.TryGetComponent(out EnergyPuzzleGate g))
        {
            gate = g;
        }
        else
        {
            gate = null;
            Debug.LogWarning("ExitAfterGateActivationSO: Target is not a valid gate.");
        }
    }

    public override bool ShouldExit(CompanionController companion, CompanionClueInteractable target)
    {
        return gate == null || gate.IsActivated() || companion.GetEnergyType() == EnergyType.None;
    }

}
