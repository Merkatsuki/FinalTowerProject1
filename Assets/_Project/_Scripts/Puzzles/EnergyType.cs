using UnityEngine;

public enum EnergyType
{
    None,
    Red,
    Blue,
    Yellow,
    Purple,
    Green
}

public static class EnergyColorMap
{
    public static Color GetColor(EnergyType type)
    {
        return type switch
        {
            EnergyType.Red => Color.red,
            EnergyType.Blue => new Color(0.2f, 0.4f, 1f),
            EnergyType.Yellow => Color.yellow,
            EnergyType.Green => Color.green,
            EnergyType.Purple => new Color(0.6f, 0.2f, 0.9f),
            EnergyType.None => Color.white,
            _ => Color.magenta
        };
    }
}


