using UnityEngine;

public interface IPerceivable
{
    Transform GetTransform();
    float GetPriority(); // Higher = more urgent
    bool IsAvailable();  // Optional flag for when it’s active/hidden/etc.
}
