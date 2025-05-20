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
    public event Action<ZoneTag> OnAngerZoneExited;

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

        bool wasInAngerZone = currentPlayerZone == ZoneTag.TheTower;

        if (!CanTriggerZone(actorTag, zone))
        {
            return;
        }

        SetZoneCooldown(actorTag, zone);

        if (actorTag == "Player")
        {
            currentPlayerZone = zone;
            OnPlayerZoneChanged?.Invoke(zone);

            if (wasInAngerZone && zone != ZoneTag.TheTower)
                OnAngerZoneExited?.Invoke(zone);
        }
        else if (actorTag == "Companion")
        {
            currentCompanionZone = zone;
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
    }

    public ZoneTag GetPlayerZone() => currentPlayerZone;
    public ZoneTag GetCompanionZone() => currentCompanionZone;
}


