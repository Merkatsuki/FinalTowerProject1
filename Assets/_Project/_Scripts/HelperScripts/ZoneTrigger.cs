using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    [SerializeField] private ZoneTag zone;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!ZoneManager.Instance) return;

        if (other.CompareTag("Player") || other.CompareTag("Companion"))
        {
            ZoneManager.Instance.NotifyZoneEntered(other.tag, zone);
        }
    }
}
