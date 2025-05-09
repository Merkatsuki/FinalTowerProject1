using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    [SerializeField] private ZoneTag zone;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!ZoneManager.Instance) return;

        if (other.CompareTag("Player") || other.CompareTag("Companion"))
        {
            Debug.Log($"ZoneTrigger: {other.tag} entered zone: {zone}");
            ZoneManager.Instance.NotifyZoneEntered(other.tag, zone);
        }
    }
}
