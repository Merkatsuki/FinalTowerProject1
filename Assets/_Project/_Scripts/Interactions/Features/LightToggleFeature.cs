using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightToggleFeature : MonoBehaviour, IInteractableFeature
{
    [SerializeField] private Light2D targetLight;

    private void Awake()
    {
        if (targetLight == null)
        {
            Transform lightChild = transform.Find("ToggleableLight");
            if (lightChild == null)
            {
                GameObject lightObj = new GameObject("ToggleableLight");
                lightObj.transform.SetParent(transform);
                lightObj.transform.localPosition = Vector3.zero;
                targetLight = lightObj.AddComponent<Light2D>();
                targetLight.intensity = 0f;
                targetLight.lightType = Light2D.LightType.Point;
                targetLight.pointLightOuterRadius = 2.5f;
                Debug.Log("[LightToggleFeature] Auto-created Toggleable Light.");
            }
            else
            {
                targetLight = lightChild.GetComponent<Light2D>();
            }
        }
    }

    public void OnInteract(IPuzzleInteractor actor)
    {
        ToggleLight();
    }

    public void ToggleLight()
    {
        if (targetLight == null) return;

        targetLight.enabled = !targetLight.enabled;
        Debug.Log("[LightToggleFeature] Toggled light state.");
    }
}