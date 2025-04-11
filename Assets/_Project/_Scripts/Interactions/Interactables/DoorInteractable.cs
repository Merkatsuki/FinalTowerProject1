using UnityEngine;

public class DoorInteractable : InteractableBase
{
    [SerializeField] private GameObject highlightVisual;
    [SerializeField] private Transform doorVisual;
    [SerializeField] private Collider2D doorCollider;
    [SerializeField] private float openDistance = 2f;
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float closeDistanceThreshold = 4f;
    [SerializeField] private float autoCloseDelay = 0.5f;

    private bool isOpen = false;
    private bool isClosing = false;
    private Vector3 closedPosition;
    private Vector3 openPosition;

    private void Start()
    {
        if (doorVisual != null)
        {
            closedPosition = doorVisual.position;
            openPosition = closedPosition + Vector3.up * openDistance;
        }

        if (playerTransform == null && GameObject.FindGameObjectWithTag("Player"))
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void Update()
    {
        if (isOpen && !isClosing && playerTransform != null)
        {
            float dist = Vector3.Distance(playerTransform.position, transform.position);
            if (dist > closeDistanceThreshold)
                StartCoroutine(CloseDoorWithDelay());
        }
    }

    public override void OnFocusEnter() => SetHighlighted(true);
    public override void OnFocusExit() => SetHighlighted(false);
    public override void SetHighlighted(bool isHighlighted)
    {
        if (highlightVisual != null)
            highlightVisual.SetActive(isHighlighted);
    }

    public override void OnInteract()
    {
        if (!isOpen)
            OpenDoor();
    }

    private void OpenDoor()
    {
        isOpen = true;
        if (doorCollider != null)
            doorCollider.enabled = false;

        if (doorVisual != null)
            StartCoroutine(SmoothMove(doorVisual, closedPosition, openPosition, openSpeed));
    }

    private System.Collections.IEnumerator CloseDoorWithDelay()
    {
        isClosing = true;
        yield return new WaitForSeconds(autoCloseDelay);

        if (doorVisual != null)
            yield return SmoothMove(doorVisual, doorVisual.position, closedPosition, openSpeed);

        if (doorCollider != null)
            doorCollider.enabled = true;

        isOpen = false;
        isClosing = false;
    }

    private System.Collections.IEnumerator SmoothMove(Transform target, Vector3 from, Vector3 to, float speed)
    {
        float elapsed = 0f;
        float duration = Vector3.Distance(from, to) / speed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            target.position = Vector3.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        target.position = to;
    }
}

