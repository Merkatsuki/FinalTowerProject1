using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ParallaxController : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform layer;
        [Range(0f, 1f)] public float parallaxFactor = 0.5f;

        [Tooltip("Which zones this layer is visible in.")]
        public List<ZoneTag> activeZones = new();

        private Vector3 initialLayerPos;

        public void CacheInitialPosition()
        {
            if (layer != null)
                initialLayerPos = layer.position;
        }

        public void UpdateLayer(float cameraDeltaX)
        {
            if (layer != null)
            {
                layer.position = new Vector3(initialLayerPos.x + cameraDeltaX * parallaxFactor, initialLayerPos.y, initialLayerPos.z);
            }
        }

        public void SetVisible(bool visible)
        {
            Debug.Log($"ParallaxLayer: Setting visibility to {visible} for layer {layer.name} in zones {string.Join(", ", activeZones)}");
            if (layer != null)
                layer.gameObject.SetActive(visible);
        }

        public bool ShouldBeVisibleInZone(ZoneTag currentZone)
        {
            return activeZones.Contains(currentZone);
        }
    }

    [SerializeField] private List<ParallaxLayer> layers = new();
    private Transform cameraTarget;
    private float initialCameraX;

    private void Start()
    {
        ZoneManager.Instance.OnPlayerZoneChanged += OnZoneChanged;
    }

    private void OnDisable()
    {
        if (ZoneManager.Instance != null)
            ZoneManager.Instance.OnPlayerZoneChanged -= OnZoneChanged;
    }

    private void OnZoneChanged(ZoneTag newZone)
    {
        Debug.Log($"ParallaxController: Zone changed to {newZone}");
        foreach (var layer in layers)
        {
            bool shouldShow = layer.ShouldBeVisibleInZone(newZone);
            layer.SetVisible(shouldShow);
        }
    }

    public void Initialize(Transform target)
    {
        cameraTarget = target;
        if (cameraTarget != null)
            initialCameraX = cameraTarget.position.x;

        foreach (var layer in layers)
        {
            layer.CacheInitialPosition();
            // Immediately show/hide for current zone
            ZoneTag currentZone = ZoneManager.Instance.GetPlayerZone();
            layer.SetVisible(layer.ShouldBeVisibleInZone(currentZone));
        }
    }

    private void LateUpdate()
    {
        if (cameraTarget == null) return;

        float deltaX = cameraTarget.position.x - initialCameraX;
        foreach (var layer in layers)
            layer.UpdateLayer(deltaX);
    }
}
