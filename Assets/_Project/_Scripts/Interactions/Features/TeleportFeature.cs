using System.Collections;
using UnityEngine;

public class TeleportFeature : FeatureBase
{
    [Header("Teleport Settings")]
    [SerializeField] private Transform teleportTarget;
    [SerializeField] private EmotionType transitionEmotion = EmotionType.Neutral;
    [SerializeField] private float teleportDelay = 0.1f;

    [Header("Transition Behavior")]
    [SerializeField] private bool requirePlayerProximity = true;
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
        if (isSolved) return;
        {
            if (teleporting || teleportTarget == null || playerTransform == null)
            {
                Debug.LogWarning("[TeleportFeature] Missing references or already teleporting.");
                return;
            }

            teleporting = true;
            StartCoroutine(HandleTeleport());
        }
    }

    private IEnumerator HandleTeleport()
    {
        yield return new WaitForSeconds(teleportDelay);

        if (TeleportTransitionManager.Instance != null)
        {
            TeleportTransitionManager.Instance.TeleportTo(teleportTarget.position, transitionEmotion);
        }

        teleporting = false;
        RunFeatureEffects();
    }

    public override void ResetPuzzleComponent()
    {
        base.ResetPuzzleComponent();
        teleporting = false;
    }
}
