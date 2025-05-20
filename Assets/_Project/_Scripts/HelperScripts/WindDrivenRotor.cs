using UnityEngine;

public class WindDrivenRotor : MonoBehaviour
{
    public bool rotateWhenActive = true;
    public float rotationMultiplier = 30f; // degrees per second per force unit
    public bool reverseDirectionIfNegative = true;

    private WindZone2D windZone;

    void Start()
    {
        windZone = GetComponentInParent<WindZone2D>();
        if (windZone == null)
            Debug.LogWarning("WindDrivenRotor: No WindZone2D found in parent!");
    }

    void FixedUpdate()
    {
        if (windZone != null && rotateWhenActive && windZone.IsActive())
        {
            float effectiveForce = windZone.GetWindForce().magnitude;
            float direction = Mathf.Sign(windZone.windDirection.x); // left/right for X winds

            float rotationSpeed = effectiveForce * rotationMultiplier * direction;

            if (!reverseDirectionIfNegative && rotationSpeed < 0)
                rotationSpeed = Mathf.Abs(rotationSpeed);

            transform.Rotate(0, 0, -rotationSpeed * Time.fixedDeltaTime);
        }
    }
}
