using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Set Flag Effect")]
public class SetFlagEffect : EffectStrategySO
{
    [SerializeField] private FlagSO flag;
    [SerializeField] private bool setValue = true;

    public void SetFlag(FlagSO flag) => this.flag = flag;
    public void SetFlagValue(bool value) => this.setValue = value;

    protected override void ApplyEffectInternal(IPuzzleInteractor actor, IWorldInteractable interactable, InteractionResult result)
    {
        if (flag != null)
        {
            FlagManager.Instance?.SetBool(flag, setValue);
            Debug.Log($"[Effect] Flag '{flag.displayName}' set to {setValue}.");
        }
        else
        {
            Debug.LogWarning("[Effect] No flag assigned in SetFlagEffect!");
        }
    }

}
