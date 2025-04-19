using UnityEngine;

public enum HoverMode
{
    FixedDirection,        // Constant offset direction (e.g., always above the object)
    RelativeToApproach,    // Offset based on the bot's approach direction
    SmartSampled           // Arc-sampled to avoid nearby obstacles and find free space
}

[CreateAssetMenu(menuName = "Companion/Hover/Hover Staging Profile")]
public class HoverStagingProfileSO : ScriptableObject
{
    [Header("Hover Logic")]
    public HoverMode mode = HoverMode.FixedDirection;
    [Tooltip("How far from the target to hover")]
    public float offsetRadius = 0.75f;
    [Tooltip("Used if mode = FixedDirection")]
    public Vector2 fixedDirection = Vector2.up;

    [Header("Smart Sampling Settings")]
    [Tooltip("Obstacles to check for when sampling hover points")]
    public LayerMask obstacleMask;
    [Tooltip("Number of positions to check around the target")]
    public int sampleRayCount = 6;
    [Tooltip("Angular range to test when sampling hover space")]
    public float sampleArcDegrees = 180f;
}
