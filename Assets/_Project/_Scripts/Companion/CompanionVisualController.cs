using UnityEngine;

public class CompanionVisualController : MonoBehaviour
{
    [Header("Visual Targets")]
    public Transform visualRoot;
    public ParticleSystem thrustParticles;

    [Header("Visual Tilt")]
    public float tiltMultiplier = 10f;
    public float tiltSpeed = 5f;

    [Header("Animation FX")]
    public float thrustMinRate = 2f;
    public float thrustMaxRate = 30f;
    public float velocityThreshold = 0.1f;

    private Vector2 lastVelocity;
    private Vector2 currentVelocity;

    public void UpdateVisuals(Vector2 velocity)
    {
        currentVelocity = velocity;

        if (visualRoot != null)
        {
            UpdateVisualTilt();
            UpdateVisualScale();
        }

        UpdateThrustParticles();

        lastVelocity = currentVelocity;
    }

    private void UpdateVisualTilt()
    {
        Vector2 accelerationVector = (currentVelocity - lastVelocity) / Time.fixedDeltaTime;
        float tiltZ = Mathf.Clamp(-accelerationVector.x * tiltMultiplier, -tiltMultiplier, tiltMultiplier);
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, tiltZ);
        visualRoot.localRotation = Quaternion.Slerp(visualRoot.localRotation, targetRotation, tiltSpeed * Time.deltaTime);
    }

    private void UpdateVisualScale()
    {
        if (currentVelocity.magnitude > velocityThreshold)
        {
            visualRoot.localScale = new Vector3(
                Mathf.Sign(currentVelocity.x),
                visualRoot.localScale.y,
                visualRoot.localScale.z);
        }
    }

    private void UpdateThrustParticles()
    {
        if (thrustParticles != null)
        {
            var emission = thrustParticles.emission;
            float t = Mathf.Clamp01(currentVelocity.magnitude);
            emission.rateOverTime = Mathf.Lerp(thrustMinRate, thrustMaxRate, t);
        }
    }
}