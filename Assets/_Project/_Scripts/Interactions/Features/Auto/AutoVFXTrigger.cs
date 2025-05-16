// AutoVFXTrigger.cs
using UnityEngine;

public class AutoVFXTrigger : AutoTriggerFeatureBase
{
    [SerializeField] private GameObject vfxPrefab;
    [SerializeField] private Transform spawnPoint;

    protected override void ExecuteTrigger()
    {
        if (vfxPrefab != null)
        {
            Instantiate(vfxPrefab, spawnPoint != null ? spawnPoint.position : transform.position, Quaternion.identity);
        }
        RunFeatureEffects();
    }
}
