using System.Collections.Generic;
using UnityEngine;

public interface IRobotPerceivable
{
    Transform GetTransform();
    float GetPriority(); // Higher = more urgent

    // Robot interaction support
    List<RobotInteractionSO> GetRobotInteractions();
}
