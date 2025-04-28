using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Spawn Prefab Effect")]
public class SpawnPrefabEffect : EffectStrategySO
{
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Vector2 spawnOffset;

    protected override void ApplyEffectInternal(IPuzzleInteractor actor, IWorldInteractable interactable, InteractionResult result)
    {
        if (prefabToSpawn != null && interactable != null)
        {
            Vector2 spawnPos = (Vector2)interactable.GetTransform().position + spawnOffset;
            Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
            Debug.Log($"[Effect] Spawned prefab at {spawnPos}");
        }
    }
}