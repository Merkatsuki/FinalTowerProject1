// Patched ZoneManager.cs with debug logging
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance { get; private set; }

    public event Action<ZoneTag> OnPlayerZoneChanged;
    public event Action<ZoneTag> OnCompanionZoneChanged;

    private ZoneTag currentPlayerZone = ZoneTag.Any;
    private ZoneTag currentCompanionZone = ZoneTag.Any;

    private Dictionary<string, Dictionary<ZoneTag, float>> zoneCooldowns = new();
    [SerializeField] private float zoneTriggerCooldown = 2.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void NotifyZoneEntered(string actorTag, ZoneTag zone)
    {
        if (actorTag == "Player" && currentPlayerZone == zone) return;
        if (actorTag == "Companion" && currentCompanionZone == zone) return;

        Debug.Log($"ZoneManager: {actorTag} entered {zone}");

        if (!CanTriggerZone(actorTag, zone))
        {
            Debug.Log($"ZoneManager: Skipping trigger for {actorTag} in {zone} (cooldown active)");
            return;
        }

        SetZoneCooldown(actorTag, zone);

        if (actorTag == "Player")
        {
            currentPlayerZone = zone;
            Debug.Log($"ZoneManager: Player zone updated to {zone}");
            OnPlayerZoneChanged?.Invoke(zone);
        }
        else if (actorTag == "Companion")
        {
            currentCompanionZone = zone;
            Debug.Log($"ZoneManager: Companion zone updated to {zone}");
            OnCompanionZoneChanged?.Invoke(zone);
        }
    }


    private bool CanTriggerZone(string actorTag, ZoneTag zone)
    {
        if (!zoneCooldowns.ContainsKey(actorTag))
            zoneCooldowns[actorTag] = new Dictionary<ZoneTag, float>();

        if (!zoneCooldowns[actorTag].ContainsKey(zone))
            return true;

        return Time.time >= zoneCooldowns[actorTag][zone];
    }

    private void SetZoneCooldown(string actorTag, ZoneTag zone)
    {
        if (!zoneCooldowns.ContainsKey(actorTag))
            zoneCooldowns[actorTag] = new Dictionary<ZoneTag, float>();

        zoneCooldowns[actorTag][zone] = Time.time + zoneTriggerCooldown;
        Debug.Log($"ZoneManager: Cooldown set for {actorTag} in {zone} until {zoneCooldowns[actorTag][zone]:F2}");
    }

    public ZoneTag GetPlayerZone() => currentPlayerZone;
    public ZoneTag GetCompanionZone() => currentCompanionZone;
}


