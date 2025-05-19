using System;
using UnityEngine;

public class GearSpinFeature : MonoBehaviour
{
    [Header("Spin Settings")]
    [SerializeField] private Transform gearVisual;
    [SerializeField] private bool alwaysSpin = false;
    public bool flipYOnPositiveRotation = false;
    public bool RotateDisBoi = false;
    [SerializeField] private float rotationModifier = 0.1f;

    [NonSerialized] public float speed_rotation = -5f;

    private bool isSpinning = false;
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;

        if (speed_rotation > 0)
        {
            // Flip Y scale if rotating clockwise due to sprite orientation
            transform.localScale = new Vector3(originalScale.x, -originalScale.y, 1f);
        }
    }

    void FixedUpdate()
    {
        if (RotateDisBoi)
        {
            isSpinning = true;
            transform.Rotate(0f, 0f, speed_rotation * rotationModifier);
        }
        else
        {
            isSpinning = false;
        }

    }

    public void SetSpinning(bool shouldSpin)
    {
        if (alwaysSpin) return;
        RotateDisBoi = shouldSpin;
    }
}
