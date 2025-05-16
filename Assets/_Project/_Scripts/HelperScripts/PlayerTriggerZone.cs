// PlayerTriggerZone.cs
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerTriggerZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<IPuzzleInteractor>(out var interactor))
            {
                foreach (var feature in GetComponents<AutoTriggerFeatureBase>())
                {
                    feature.OnPlayerEnterZone(interactor);
                }
            }
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var feature in GetComponents<AutoTriggerFeatureBase>())
            {
                feature.OnPlayerExitZone();
            }
        }
    }

}
