using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Entry/EntryCooldown")]
public class EntryCooldownStrategySO : EntryStrategySO
{
    [SerializeField] private float cooldownDuration = 5f;

    private float lastEntryTime = -9999f;

    public override bool CanEnter(IPuzzleInteractor actor, IWorldInteractable interactable)
    {
        return Time.time - lastEntryTime >= cooldownDuration;
    }

    public override void OnEnter(IPuzzleInteractor actor, IWorldInteractable interactable)
    {
        lastEntryTime = Time.time;
    }

    public void SetCooldown(float seconds)
    {
        cooldownDuration = seconds;
    }
}