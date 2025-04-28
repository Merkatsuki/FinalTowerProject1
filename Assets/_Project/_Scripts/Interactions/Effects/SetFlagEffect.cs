using UnityEngine;

public class SetFlagEffect : EffectStrategySO
{
    [SerializeField] private FlagSO flag;
    [SerializeField] private bool setValue = true;
    [SerializeField] private bool onlyOnSuccess = true;

    public void SetFlag(FlagSO flag) => this.flag = flag;
    public void SetFlagValue(bool value) => this.setValue = value;
    public void SetOnlyOnSuccess(bool value) => this.onlyOnSuccess = value;

    protected override void ApplyEffectInternal(IPuzzleInteractor actor, IWorldInteractable interactable, InteractionResult result)
    {
        if (flag != null)
        {
            PuzzleManager.Instance?.SetFlag(flag, setValue);
            Debug.Log($"[Effect] Flag '{flag.flagName}' set to {setValue}.");
        }
        else
        {
            Debug.LogWarning("[Effect] No flag assigned in SetFlagEffect!");
        }
    }

}
