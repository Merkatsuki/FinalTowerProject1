
using UnityEngine;

public static class EmotionColorMap
{
    public static Color GetColor(EmotionTag tag)
    {
        return tag switch
        {
            EmotionTag.Joy => new Color(1f, 0.85f, 0.2f),     // Yellow-gold
            EmotionTag.Anger => new Color(0.85f, 0.2f, 0.2f),   // Red-orange
            EmotionTag.Sadness => new Color(0.3f, 0.5f, 1f),    // Blue
            EmotionTag.Neutral => Color.gray,
            EmotionTag.Any => Color.white,
            _ => Color.white
        };
    }
}

