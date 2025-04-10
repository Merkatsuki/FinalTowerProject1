using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    [Header("Cinemachine References")]
    [SerializeField] private CinemachineCamera virtualCamera;

    [Header("Zoom Settings")]
    [SerializeField] private float defaultZoom = 5f;
    [SerializeField] private float zoomSpeed = 1f;

    [Header("Shake Settings")]
    [SerializeField] private float shakeFrequency = 2f;

    private Coroutine zoomCoroutine;
    private Coroutine shakeCoroutine;

    private CinemachinePositionComposer positionComposer;
    private CinemachineBasicMultiChannelPerlin noise;

    private void Awake()
    {
        if (virtualCamera == null)
        {
            Debug.LogError("CameraController: Virtual Camera reference is missing.");
            return;
        }

        positionComposer = virtualCamera.GetComponent<CinemachinePositionComposer>();
        noise = virtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();

        if (positionComposer == null)
            Debug.LogWarning("CameraController: PositionComposer component is missing.");

        if (noise == null)
            Debug.LogWarning("CameraController: Perlin component is missing.");
    }

    public void FollowTarget(Transform target)
    {
        virtualCamera.Follow = target;
        virtualCamera.LookAt = target;
    }

    public void FocusOn(Transform target, float blendTime = 1f)
    {
        virtualCamera.Follow = target;
        if (positionComposer != null)
            positionComposer.Damping.x = blendTime;
    }

    public void SetZoom(float newZoom, float duration = 0f)
    {
        if (zoomCoroutine != null)
            StopCoroutine(zoomCoroutine);

        if (duration <= 0f)
        {
            positionComposer.CameraDistance = newZoom;
        }
        else
        {
            zoomCoroutine = StartCoroutine(SmoothZoom(newZoom, duration));
        }
    }

    private IEnumerator SmoothZoom(float targetZoom, float duration)
    {
        float startZoom = positionComposer.CameraDistance;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            positionComposer.CameraDistance = Mathf.Lerp(startZoom, targetZoom, elapsed / duration);
            yield return null;
        }

        positionComposer.CameraDistance = targetZoom;
    }

    public void ShakeCamera(float intensity = 1f, float duration = 0.5f)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(DoCameraShake(intensity, duration));
    }

    private IEnumerator DoCameraShake(float intensity, float duration)
    {
        if (noise != null)
        {
            noise.AmplitudeGain = intensity;
            noise.FrequencyGain = shakeFrequency;

            yield return new WaitForSeconds(duration);

            noise.AmplitudeGain = 0f;
        }
    }

    public void SetConfinerBounds(Collider2D bounds)
    {
        var confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
        if (confiner != null)
        {
            confiner.BoundingShape2D = bounds;
            confiner.InvalidateBoundingShapeCache();
        }
        else
        {
            Debug.LogWarning("CameraController: Confiner2D component is missing.");
        }
    }

    public void ResetZoom()
    {
        SetZoom(defaultZoom, 0.5f);
    }
}
