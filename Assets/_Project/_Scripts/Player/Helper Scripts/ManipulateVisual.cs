using UnityEngine;

public class ManipulateVisual : MonoBehaviour
{
    [Header("Rotation")]
    public float rotateSpeed = 60f;

    [Header("Pulse")]
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.1f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Rotate continuously
        transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);

        // Pulse scale
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = originalScale * pulse;
    }
}
