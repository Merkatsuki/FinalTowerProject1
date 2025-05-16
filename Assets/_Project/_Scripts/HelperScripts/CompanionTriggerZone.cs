// CompanionTriggerZone.cs
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CompanionTriggerZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Companion"))
        {
            if (other.TryGetComponent<IPuzzleInteractor>(out var interactor))
            {
                foreach (var feature in GetComponents<AutoTriggerFeatureBase>())
                {
                    feature.OnPlayerEnterZone(interactor); // Rename to OnCompanionEnterZone if needed
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Companion"))
        {
            foreach (var feature in GetComponents<AutoTriggerFeatureBase>())
            {
                feature.OnPlayerExitZone(); // Or OnCompanionExitZone
            }
        }
    }
}
