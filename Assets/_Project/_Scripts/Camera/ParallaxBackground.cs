using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform layerTransform;
        public Vector2 parallaxMultiplier = new Vector2(0.5f, 0.5f); // X/Y parallax speeds
    }

    public ParallaxLayer[] layers;
    public Transform target; // Will or Robot
    private Vector3 previousTargetPosition;

    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("ParallaxBackground: No target set!");
            enabled = false;
            return;
        }
        previousTargetPosition = target.position;
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = target.position - previousTargetPosition;

        foreach (var layer in layers)
        {
            if (layer.layerTransform == null) continue;

            Vector3 newPosition = layer.layerTransform.position;
            newPosition += new Vector3(
                deltaMovement.x * layer.parallaxMultiplier.x,
                deltaMovement.y * layer.parallaxMultiplier.y,
                0f
            );
            layer.layerTransform.position = newPosition;
        }

        previousTargetPosition = target.position;
    }

    // For switching control during gameplay
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        previousTargetPosition = newTarget.position;
    }
}
