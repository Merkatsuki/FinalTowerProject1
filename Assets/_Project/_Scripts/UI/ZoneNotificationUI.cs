using UnityEngine;
using TMPro;
using System.Collections;

public class ZoneNotificationUI : MonoBehaviour
{
    [SerializeField] private TMP_Text zoneText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float displayTime = 3f;
    [SerializeField] private float fadeTime = 0.5f;

    private Coroutine displayRoutine;

    private void OnEnable()
    {
        if (ZoneManager.Instance != null)
        {
            Debug.Log("ZoneNotificationUI: Subscribing to OnPlayerZoneChanged");
            ZoneManager.Instance.OnPlayerZoneChanged += ShowZone;
        }
    }

    private void OnDisable()
    {
        if (ZoneManager.Instance != null)
        {
            Debug.Log("ZoneNotificationUI: Unsubscribing from OnPlayerZoneChanged");
            ZoneManager.Instance.OnPlayerZoneChanged -= ShowZone;
        }
    }

    private void ShowZone(ZoneTag zone)
    {
        Debug.Log($"ZoneNotificationUI: ShowZone called for zone: {zone}");
        string displayName = GetZoneName(zone);
        if (displayRoutine != null) StopCoroutine(displayRoutine);
        displayRoutine = StartCoroutine(DisplayZoneName(displayName));
    }

    private IEnumerator DisplayZoneName(string name)
    {
        Debug.Log($"ZoneNotificationUI: Displaying zone name: {name}");
        zoneText.text = name;
        canvasGroup.alpha = 0;

        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = t / fadeTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(displayTime);

        t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1f - (t / fadeTime);
            yield return null;
        }

        canvasGroup.alpha = 0;
    }

    private string GetZoneName(ZoneTag tag)
    {
        Debug.Log($"ZoneNotificationUI: GetZoneName for tag: {tag}");
        return tag switch
        {
            ZoneTag.Hub => "Hub",
            ZoneTag.Junkyard => "Junkyard",
            ZoneTag.Elevator => "Elevator",
            ZoneTag.TheTower => "The Tower",
            ZoneTag.Greenhouse => "Greenhouse",
            ZoneTag.TheChip => "The Chip",
            _ => string.Empty
        };
    }
}
