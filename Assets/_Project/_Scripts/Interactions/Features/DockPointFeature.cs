using DG.Tweening;
using UnityEngine;

public class DockPointFeature : FeatureBase
{
    [SerializeField] private Transform dockTarget;

    public Vector3 GetDockPosition()
    {
        return dockTarget != null ? dockTarget.position : transform.position;
    }

    public override void OnInteract(IPuzzleInteractor interactor)
    {
        var companion = ReferenceManager.Instance?.Companion;
        if (companion == null)
        {
            Debug.LogWarning("[DockPointFeature] Companion not found.");
            return;
        }

        companion.DockTo(new DockConfig(
            GetDockPosition(),
            1.5f,
            () => isComplete = true
        ));
    }

    public void TriggerDock()
    {
        OnInteract(null);
    }
}