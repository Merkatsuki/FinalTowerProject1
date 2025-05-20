using UnityEngine;

public class AngerPuzzleRoomManager : MonoBehaviour
{
    public static AngerPuzzleRoomManager Instance { get; private set; }

    [Header("Camera Control")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Transform cameraFocusPoint;
    [SerializeField] private Transform playerFollowTarget;
    [SerializeField] private float cameraZoomOverride = 15f;
    [SerializeField] private float zoomDuration = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        ZoneManager.Instance.OnPlayerZoneChanged += OnPlayerZoneChanged;
        ZoneManager.Instance.OnAngerZoneExited += OnAngerZoneExited;
    }

    public void EnterPuzzleZone()
    {
        if (cameraFocusPoint != null)
        {
            cameraController.FollowActiveCameraTarget(cameraFocusPoint);
            cameraController.SetZoom(cameraZoomOverride, zoomDuration);
        }
    }

    public void ExitPuzzleZone()
    {
        if (playerFollowTarget != null)
        {
            cameraController.SetCameraMode(false); 
            cameraController.FollowActiveCameraTarget(playerFollowTarget);
            cameraController.SetZoom(cameraController.DefaultZoom, zoomDuration);
        }
    }

    private void OnPlayerZoneChanged(ZoneTag newZone)
    {
        if (newZone == ZoneTag.TheTower)
        {
            EnterPuzzleZone();
        }
    }

    private void OnAngerZoneExited(ZoneTag exitedTo)
    {
        ExitPuzzleZone();
    }

}
