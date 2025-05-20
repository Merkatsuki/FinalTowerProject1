using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    [Header("Cinemachine References")]
    [SerializeField] private CinemachineCamera playerVirtualCamera;
    [SerializeField] private CinemachineCamera companionVirtualCamera;

    [Header("Parallax")]
    [SerializeField] private ParallaxController parallaxController;

    [Header("Zoom Settings")]
    [SerializeField] private float defaultZoom = 5.5f;
    [SerializeField] private float zoomSpeed = 1f;

    [Header("Shake Settings")]
    [SerializeField] private float shakeFrequency = 2f;

    [Header("Command Settings")]
    [SerializeField] private CanvasGroup commandOverlay;

    [Header("Damping Settings")]
    private Vector3 originalDamping;

    private Transform lastFollowTarget;

    private Coroutine zoomCoroutine;

    private CinemachinePositionComposer positionComposer;
    private CinemachineBasicMultiChannelPerlin noise;

    public float DefaultZoom => defaultZoom;

    private void Awake()
    {
        if (playerVirtualCamera == null)
        {
            Debug.LogError("CameraController: Virtual Camera reference is missing.");
            return;
        }

        positionComposer = playerVirtualCamera.GetComponent<CinemachinePositionComposer>();
        noise = playerVirtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();

        if (positionComposer == null)
            Debug.LogWarning("CameraController: PositionComposer component is missing.");

        if (noise == null)
            Debug.LogWarning("CameraController: Perlin component is missing.");
    }

    private void LateUpdate()
    {
        if (parallaxController != null)
        {
            Transform follow = playerVirtualCamera.Follow;
            if (follow != null && follow != lastFollowTarget)
            {
                parallaxController.Initialize(follow);
                lastFollowTarget = follow;
            }
        }
    }

    public void SetZoom(float newZoom, float duration)
    {
        if (zoomCoroutine != null)
            StopCoroutine(zoomCoroutine);

        zoomCoroutine = StartCoroutine(SmoothZoom(newZoom, duration));
    }

    private IEnumerator SmoothZoom(float targetZoom, float duration)
    {
        float startZoom = playerVirtualCamera.Lens.OrthographicSize;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            playerVirtualCamera.Lens.OrthographicSize = Mathf.Lerp(startZoom, targetZoom, t);
            yield return null;
        }

        playerVirtualCamera.Lens.OrthographicSize = targetZoom;
    }

    public void FollowActiveCameraTarget(Transform target)
    {
        CinemachineCamera activeCam = IsInCommandMode() ? companionVirtualCamera : playerVirtualCamera;

        if (activeCam == null)
        {
            Debug.LogWarning("[CameraController] No active virtual camera found.");
            return;
        }

        activeCam.Follow = target;
        activeCam.LookAt = target;

        if (parallaxController != null)
            parallaxController.Initialize(target);

        Debug.Log($"[CameraController] Active camera now following: {target.name}");
    }

    private bool IsInCommandMode()
    {
        return companionVirtualCamera != null && companionVirtualCamera.Priority > playerVirtualCamera.Priority;
    }

    public void SetCameraMode(bool isCommandMode)
    {
        // Block camera switch if we're in the Anger zone
        if (ZoneManager.Instance.GetPlayerZone() == ZoneTag.TheTower)
        {
            Debug.Log("[CameraController] Anger zone active — skipping camera mode switch.");
            return;
        }

        if (playerVirtualCamera != null && companionVirtualCamera != null)
        {
            playerVirtualCamera.Priority = isCommandMode ? 0 : 10;
            companionVirtualCamera.Priority = isCommandMode ? 10 : 0;
        }
    }

    public void SetCommandOverlayActive(bool isActive)
    {
        commandOverlay.alpha = isActive ? 1f : 0f;
        commandOverlay.interactable = false;
        commandOverlay.blocksRaycasts = false;
    }

    public void SnapToTargetImmediately()
    {
        if (positionComposer != null)
        {
            originalDamping = positionComposer.Damping;
            positionComposer.Damping = Vector3.zero; // Disable all smoothing
        }
    }

    public void RestoreCameraDamping()
    {
        if (positionComposer != null)
        {
            positionComposer.Damping = originalDamping;
        }
    }


}