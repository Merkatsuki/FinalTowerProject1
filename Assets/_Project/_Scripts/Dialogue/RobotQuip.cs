using UnityEngine;

[CreateAssetMenu(fileName = "New Robot Quip", menuName = "Dialogue/Robot Quip")]
public class RobotQuip : ScriptableObject
{
    [TextArea]
    public string quipText;

    public QuipTriggerType triggerType = QuipTriggerType.AmbientRandom;
    public EmotionTag emotionTag = EmotionTag.Neutral;
    public ZoneTag zoneTag = ZoneTag.Any;
    public QuipTone tone = QuipTone.Witty;

    public float weight = 1f; 
    public bool repeatable = true;
    public int repeatAfterQuips = 5;
    public int useLimit = -1; // -1 means unlimited
}


public enum QuipTriggerType 
{ 
    OnIdle, 
    OnDirectionSpam, 
    OnPuzzleSolve, 
    OnPuzzleFail, 
    OnZoneEnter, 
    OnEmotionSwitch,
    OnInteractableApproach, 
    OnMemoryFragmentPickup, 
    OnStoryPagePickup,
    AmbientRandom 
}
public enum EmotionTag 
{ 
    Any,
    Neutral, 
    Joy, 
    Anger, 
    Sadness 
}
public enum ZoneTag 
{ 
    Any, 
    Hub, 
    Junkyard, 
    JoyZone,
    AngerZone, 
    SadnessZone, 
    FinalMemory 
}
public enum QuipTone 
{ 
    Any,
    Witty, 
    Glitched, 
    Ominous,
    Reflective, 
    Sarcastic,
    Poetic 
}
