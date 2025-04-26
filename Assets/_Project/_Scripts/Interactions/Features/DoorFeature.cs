using UnityEngine;

public class DoorFeature : MonoBehaviour, IInteractableFeature
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private float closeDelay = 2.0f;

    private bool isOpen = false;

    private void Awake()
    {
        if (doorAnimator == null)
        {
            doorAnimator = GetComponent<Animator>();
            if (doorAnimator == null)
            {
                doorAnimator = gameObject.AddComponent<Animator>();
                Debug.LogWarning("[DoorFeature] No Animator found. Added default Animator component. Assign a Controller manually!");
            }
        }
    }

    public void OnInteract(IPuzzleInteractor actor)
    {
        OpenDoor();
    }

    public void OpenDoor()
    {
        if (isOpen) return;

        isOpen = true;
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Open");
            Invoke(nameof(CloseDoor), closeDelay);
            Debug.Log("[DoorFeature] Door opened.");
        }
    }

    private void CloseDoor()
    {
        if (!isOpen) return;

        isOpen = false;
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Close");
            Debug.Log("[DoorFeature] Door closed.");
        }
    }

    public bool IsOpen() => isOpen;
}