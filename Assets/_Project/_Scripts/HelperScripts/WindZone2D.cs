using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Collider2D))]
public class WindZone2D : MonoBehaviour
{
    public enum WindStartDirection
    {
        Right,
        Up,
        Left,
        Down
    }

    public Vector2 windDirection = Vector2.right;
    public float force = 5f;

    [Header("Timing")]
    public bool useTimedCycle = false;
    public float onTime = 2f;
    public float offTime = 2f;

    private float cycleTimer = 0f;
    private bool windIsOn = true;

    [Header("Visual & Audio Feedback")]
    [SerializeField] private ParticleSystem windParticles;
    [SerializeField] private Light2D windLight;          
    [SerializeField] private Color windOnColor = Color.red;
    [SerializeField] private Color windOffColor = Color.green;
    [SerializeField] private Color windWarningColor = Color.yellow;

    [SerializeField] private float warningDuration = 2f;
    [SerializeField] private AudioSource windSoundLoop;
    [SerializeField] private AudioSource windWarningSound;
    [SerializeField] private Transform rotorVisual;

    [Header("Mode Settings")]
    public bool useDirectionalParticles = false;
    [SerializeField] private WindStartDirection startDirection = WindStartDirection.Right;
    [SerializeField] private ParticleSystem particleRight;
    [SerializeField] private ParticleSystem particleUp;
    [SerializeField] private ParticleSystem particleLeft;
    [SerializeField] private ParticleSystem particleDown;

    private ParticleSystem currentParticles;
    private bool isWarningPhase = false;
    private Coroutine pulseCoroutine;

    private void Start()
    {
        SetInitialDirectionFromEnum();
        ApplyVisuals();
        if (useDirectionalParticles) UpdateActiveParticleSystem();
    }

    private void Update()
    {
        HandleCycleTiming();

        if (useDirectionalParticles)
        {
            HandleDirectionalParticleSystem();
        }
        else
        {
            HandleSingleParticleSystem();
        }
    }

    private void SetInitialDirectionFromEnum()
    {
        switch (startDirection)
        {
            case WindStartDirection.Right:
                windDirection = Vector2.right;
                break;
            case WindStartDirection.Up:
                windDirection = Vector2.up;
                break;
            case WindStartDirection.Left:
                windDirection = Vector2.left;
                break;
            case WindStartDirection.Down:
                windDirection = Vector2.down;
                break;
        }

        ApplyRotationVisual(); // Face rotor
    }


    public void ToggleActive()
    {
        windIsOn = !windIsOn;
        ApplyVisuals();
        if (useDirectionalParticles) UpdateActiveParticleSystem();
    }

    private void HandleDirectionalParticleSystem()
    {
        if (currentParticles == null) return;

        if (IsActive())
        {
            if (!currentParticles.isPlaying)
                currentParticles.Play();
        }
        else
        {
            if (currentParticles.isPlaying)
                currentParticles.Stop();
        }
    }

    private void HandleSingleParticleSystem()
    {
        if (windParticles == null) return;

        var main = windParticles.main;
        var velocityOverLifetime = windParticles.velocityOverLifetime;
        var forceOverLifetime = windParticles.forceOverLifetime;

        if (IsActive())
        {
            if (!windParticles.isPlaying) windParticles.Play();

            Vector3 dir = new Vector3(windDirection.x, windDirection.y, 0).normalized;
            velocityOverLifetime.enabled = true;
            velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
            velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(dir.x * force);
            velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(dir.y * force);

            forceOverLifetime.enabled = false;

            var emission = windParticles.emission;
            emission.rateOverTime = force * 10f;

            var noise = windParticles.noise;
            noise.enabled = true;
            noise.strength = force * 0.2f;
            noise.scrollSpeed = force;
        }
        else
        {
            if (windParticles.isPlaying) windParticles.Stop();
        }
    }


    private void HandleCycleTiming()
    {
        if (isWarningPhase && windLight != null)
        {
            windLight.intensity = 1.5f + Mathf.Sin(Time.time * 10f) * 0.5f;
        }
        else if (windLight != null)
        {
            windLight.intensity = 1.5f;
        }

        if (!useTimedCycle) return;

        cycleTimer += Time.deltaTime;

        if (windIsOn)
        {
            if (cycleTimer >= onTime)
            {
                windIsOn = false;
                isWarningPhase = false;
                cycleTimer = 0f;
                ApplyVisuals();
            }
        }
        else
        {
            if (!isWarningPhase && cycleTimer >= (offTime - warningDuration))
            {
                isWarningPhase = true;
                ApplyVisuals();

                if (windWarningSound != null && !windWarningSound.isPlaying)
                    windWarningSound.Play();
            }

            if (cycleTimer >= offTime)
            {
                windIsOn = true;
                isWarningPhase = false;
                cycleTimer = 0f;
                ApplyVisuals();

                if (windSoundLoop != null && !windSoundLoop.isPlaying)
                    windSoundLoop.Play();
            }
        }
    }

    private void ApplyVisuals()
    {
        if (!windIsOn && windParticles != null && windParticles.isPlaying)
            windParticles.Stop();

        if (windLight != null)
        {
            if (isWarningPhase)
            {
                windLight.color = windWarningColor;
            }
            else if (windIsOn)
            {
                windLight.color = windOnColor;
            }
            else
            {
                windLight.color = windOffColor;
            }
        }

        if (windSoundLoop != null)
        {
            if (windIsOn && !windSoundLoop.isPlaying)
                windSoundLoop.Play();
            else if (!windIsOn && windSoundLoop.isPlaying)
                windSoundLoop.Stop();
        }
    }

    public void ApplyRotationVisual()
    {
        if (rotorVisual == null)
        {
            Debug.LogWarning("[WindZone2D] rotorVisual not assigned, skipping visual rotation.");
            return;
        }

        float angle = Mathf.Atan2(windDirection.y, windDirection.x) * Mathf.Rad2Deg;
        rotorVisual.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void UpdateActiveParticleSystem() => UpdateParticleSelection();

    private void UpdateParticleSelection()
    {
        if (currentParticles != null && currentParticles.isPlaying)
        {
            Debug.Log($"[WindZone2D] Stopping previous particle system: {currentParticles.name}");
            currentParticles.Stop();
        }

        Vector2 dir = windDirection.normalized;
        Debug.Log($"[WindZone2D] Normalized windDirection = {dir}");

        float rightDot = Vector2.Dot(dir, Vector2.right);
        float leftDot = Vector2.Dot(dir, Vector2.left);
        float upDot = Vector2.Dot(dir, Vector2.up);
        float downDot = Vector2.Dot(dir, Vector2.down);

        Debug.Log($"[WindZone2D] Dot Products - Right: {rightDot}, Left: {leftDot}, Up: {upDot}, Down: {downDot}");

        if (rightDot > 0.9f)
        {
            currentParticles = particleRight;
            Debug.Log("[WindZone2D] Selected particleRight");
        }
        else if (leftDot > 0.9f)
        {
            currentParticles = particleLeft;
            Debug.Log("[WindZone2D] Selected particleLeft");
        }
        else if (upDot > 0.9f)
        {
            currentParticles = particleUp;
            Debug.Log("[WindZone2D] Selected particleUp");
        }
        else if (downDot > 0.9f)
        {
            currentParticles = particleDown;
            Debug.Log("[WindZone2D] Selected particleDown");
        }
        else
        {
            currentParticles = null;
            Debug.LogWarning($"[WindZone2D] No matching directional particle system found for direction: {dir}");
        }

        if (currentParticles != null)
            Debug.Log($"[WindZone2D] Final particle selected: {currentParticles.name}");
    }

    public void PulseLight(float duration = 0.9f, float pulseAmount = 4f)
    {
        if (windLight == null) return;

        if (pulseCoroutine != null)
            StopCoroutine(pulseCoroutine);

        pulseCoroutine = StartCoroutine(PulseRoutine(duration, pulseAmount));
    }

    private IEnumerator PulseRoutine(float duration, float pulseAmount)
    {
        float original = windLight.intensity;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float pulse = Mathf.Sin(t * Mathf.PI / duration) * pulseAmount;
            windLight.intensity = original + pulse;
            yield return null;
        }

        windLight.intensity = original;
    }

    public bool IsActive() => windIsOn;

    public Vector2 GetWindForce()
    {
        return IsActive() ? windDirection.normalized * force : Vector2.zero;
    }
}

[System.Serializable]
public class WindZoneAction
{
    public WindZone2D windZone;
    public ActionType action;

    public enum ActionType { Toggle, Rotate90 }
}