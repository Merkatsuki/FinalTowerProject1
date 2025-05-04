using System.Collections;
using UnityEngine;

public class OneWayPlatformController : MonoBehaviour
{
    [SerializeField] private Collider2D platformCollider;
    [SerializeField] private float disableDuration = 0.2f;

    public void DropThrough()
    {
        StartCoroutine(DisableTemporarily());
    }

    private IEnumerator DisableTemporarily()
    {
        platformCollider.enabled = false;
        yield return new WaitForSeconds(disableDuration);
        platformCollider.enabled = true;
    }
}
