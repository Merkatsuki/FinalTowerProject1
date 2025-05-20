using UnityEngine;

public class ZoneCameraTrigger : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Transform cameraFocusPoint;
    [SerializeField] private float zoom = 4.5f;
    [SerializeField] private float zoomDuration = 0.75f;
    [SerializeField] private bool triggerOnce = true;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered && triggerOnce) return;
        if (!other.CompareTag("Player")) return;

        cameraController.FollowActiveCameraTarget(cameraFocusPoint);
        cameraController.SetZoom(zoom, zoomDuration);

        triggered = true;
        Debug.Log("[ZoneCameraTrigger] Sadness Zone camera override activated.");
    }
}

