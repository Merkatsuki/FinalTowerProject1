using System.Collections;
using UnityEngine;

public enum TeleportMode
{
    SceneChange,
    PositionOnly,
    SadnessPuzzleRoom
}

public class TeleportFeature : FeatureBase
{
    private static float lastGlobalTeleportTime = -10f;
    private const float globalPortalCooldown = 1f;

    [Header("Teleport Settings")]
    [SerializeField] private float teleportDelay = 0.1f;
      
    [SerializeField] private TeleportMode teleportMode = TeleportMode.PositionOnly;
    [SerializeField] private EmotionType transitionEmotion = EmotionType.Neutral;

    [Header("Shared Portal Info")]
    [SerializeField] private PortalSide thisDoorSide;

    [Header("Exit Configuration")]
    [SerializeField] private Transform teleportEndLocation;
    [SerializeField] private bool faceRightOnExit = true;

    [Header("Transition Behavior")]
    //[SerializeField] private bool requirePlayerProximity = true;
    [SerializeField] private Transform playerTransform;


    private bool teleporting = false;

    protected override void Awake()
    {
        base.Awake();
        if (playerTransform == null && GameController.Instance != null)
        {
            playerTransform = GameController.Instance.transform; // Or assign via inspector
        }
    }

    public override void OnInteract(IPuzzleInteractor actor)
    {
        if (isSolved || teleporting)
            return;

        if (Time.time < lastGlobalTeleportTime + globalPortalCooldown)
        {
            Debug.Log("[TeleportFeature] Global cooldown active. Teleport blocked.");
            return;
        }

        teleporting = true;
        StartCoroutine(HandleTeleport());
    }

    private IEnumerator HandleTeleport()
    {
        yield return new WaitForSeconds(teleportDelay);

        switch (teleportMode)
        {
            case TeleportMode.PositionOnly:
                if (teleportEndLocation != null)
                {
                    TeleportTransitionManager.Instance.TeleportTo(teleportEndLocation.position, transitionEmotion, faceRightOnExit);
                }
                else
                {
                    Debug.LogWarning("[TeleportFeature] PositionOnly mode requires a teleportEndLocation.");
                }
                break;

            case TeleportMode.SadnessPuzzleRoom:
                SadnessPuzzleRoom currentRoom = SadnessPuzzleRoomManager.Instance.GetCurrentRoom();
                if (currentRoom == null)
                {
                    Debug.LogWarning("[TeleportFeature] No current room assigned.");
                    break;
                }

                PortalLink link = currentRoom.GetLink(thisDoorSide);
                if (link == null || link.targetRoom == null)
                {
                    Debug.LogWarning($"[TeleportFeature] No valid portal link from side {thisDoorSide}.");
                    break;
                }

                TeleportFeature destinationFeature = link.targetRoom.GetFeatureForSide(link.targetSide);
                if (destinationFeature == null)
                {
                    Debug.LogWarning($"[TeleportFeature] No destination feature found for side {link.targetSide} in {link.targetRoom.name}");
                    break;
                }

                Debug.Log($"[TeleportFeature] Teleporting from {currentRoom.name}.{thisDoorSide}");

                // DEFER room switch until mid-transition
                TeleportTransitionManager.Instance.TeleportTo(
                    destinationFeature.teleportEndLocation.position,
                    transitionEmotion,
                    destinationFeature.faceRightOnExit,
                    link.targetRoom
                );
                break;

            case TeleportMode.SceneChange:
                Debug.LogWarning("[TeleportFeature] SceneChange not yet implemented.");
                break;
        }

        lastGlobalTeleportTime = Time.time;
        teleporting = false;
        RunFeatureEffects();
    }

    public override void ResetPuzzleComponent()
    {
        base.ResetPuzzleComponent();
        teleporting = false;
    }

    public PortalSide GetSide() => thisDoorSide;
    public Transform GetTeleportEndLocation() => teleportEndLocation;
    public bool GetFaceRight() => faceRightOnExit;
}
