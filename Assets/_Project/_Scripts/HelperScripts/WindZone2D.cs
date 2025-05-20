using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Collider2D))]
public class WindZone2D : MonoBehaviour
{
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

    private bool isWarningPhase = false;

    private void Start()
    {
        ApplyVisuals();
    }

    private void Update()
    {
        HandleCycleTiming();


        if (windParticles != null)
        {
            var main = windParticles.main;
            var velocityOverLifetime = windParticles.velocityOverLifetime;
            var forceOverLifetime = windParticles.forceOverLifetime;

            if (IsActive())
            {
                if (!windParticles.isPlaying) windParticles.Play();

                // Directional movement
                Vector3 dir = new Vector3(windDirection.x, windDirection.y, 0).normalized;

                // Adjust particle movement visually
                velocityOverLifetime.enabled = true;
                velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
                velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(dir.x * force);
                velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(dir.y * force);

                // Optional: turbulence or force addition
                forceOverLifetime.enabled = false;

                // Emission rate scaling
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


    public bool IsActive() => !useTimedCycle || windIsOn;

    public Vector2 GetWindForce()
    {
        return IsActive() ? windDirection.normalized * force : Vector2.zero;
    }
}
