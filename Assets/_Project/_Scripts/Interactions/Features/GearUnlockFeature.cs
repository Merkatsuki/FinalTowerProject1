using UnityEngine;

public class GearUnlockFeature : FeatureBase
{
    [SerializeField] private ElevatorPlatformFeature targetElevator;
    [SerializeField] private Transform lockAnchorToUnlock;
    [SerializeField] private bool autoActivateOnStart;
    [SerializeField] private bool toggleMode = false;

    private bool unlocked = false;

    private void Start()
    {
        if (autoActivateOnStart) AutoUnlock();

    }
    public override void OnInteract(IPuzzleInteractor interactor)
    {
        if (targetElevator == null || lockAnchorToUnlock == null) return;

        if (!toggleMode)
        {
            if (unlocked) return;
            targetElevator.UnlockGearAt(lockAnchorToUnlock);
            unlocked = true;
        }
        else
        {
            unlocked = !unlocked;
            targetElevator.SetGearLockState(lockAnchorToUnlock, unlocked);
        }

        NotifyPuzzleInteractionSuccess();
        RunFeatureEffects(interactor);
    }
    public void AutoUnlock() // Optional for light-based activation
    {
        if (unlocked || targetElevator == null || lockAnchorToUnlock == null) return;

        targetElevator.UnlockGearAt(lockAnchorToUnlock);
        unlocked = true;

        NotifyPuzzleInteractionSuccess();
        RunFeatureEffects();
    }
}
