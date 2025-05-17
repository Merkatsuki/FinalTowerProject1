[System.Serializable]
public class PortalLink
{
    public PortalSide sourceSide; // LB, LT, RB, RT
    public SadnessPuzzleRoom targetRoom;
    public PortalSide targetSide;
}

public enum PortalSide
{
    LB = 0, // Left Bottom
    LT = 1, // Left Top
    RB = 2, // Right Bottom
    RT = 3  // Right Top
}