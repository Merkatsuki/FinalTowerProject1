using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DockingZone : MonoBehaviour
{
    [Tooltip("Optional point where the robot should position itself.")]
    public Transform dockingPoint;

    public void Dock(CompanionController companion)
    {
        Vector3 target = dockingPoint ? dockingPoint.position : transform.position;
        companion.transform.position = target;
        Debug.Log("Companion docked at: " + name);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(dockingPoint ? dockingPoint.position : transform.position, 0.3f);
    }
}