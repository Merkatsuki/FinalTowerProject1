using System.Collections.Generic;
using UnityEngine;

public interface IRobotPerceivable
{
    Transform GetTransform();
    float GetPriority(); // Higher = more urgent
    bool IsAvailable();  // Optional flag for when it’s active/hidden/etc.

    // Robot interaction support
    List<RobotInteractionSO> GetRobotInteractions();
}
