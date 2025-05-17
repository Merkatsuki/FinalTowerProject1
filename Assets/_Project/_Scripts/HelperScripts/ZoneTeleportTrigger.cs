using UnityEngine;

public class ZoneTeleportTrigger : MonoBehaviour
{
    public Transform targetLocation;
    public EmotionType transitionEmotion;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TeleportTransitionManager.Instance.TeleportTo(targetLocation.position, transitionEmotion, true);
        }
    }
}
