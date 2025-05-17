using DG.Tweening;
using Momentum;
using System.Collections;
using UnityEngine;
using static Momentum.PlayerChecks;

public class TeleportTransitionManager : MonoBehaviour, IGameStateListener
{
    public static TeleportTransitionManager Instance { get; private set; }

    [SerializeField] private Transform playerTransform;
    [SerializeField] private CameraController cameraController;

    [Header("Fade Canvas")]
    [SerializeField] private CanvasGroup screenFade;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Visuals & Audio")]
    [SerializeField] private AudioClip teleportSound;
    [SerializeField] private GameObject teleportVFX;
    [SerializeField] private Transform vfxSpawnParent;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
    }
    private void Start()
    {
        GameStateManager.Instance.RegisterListener(this);
        audioSource = GetComponent<AudioSource>();
    }

    public void OnGameStateChanged(GameState newState)
    {
        // Optional: fade out HUD, disable UI inputs, etc.
    }

    public void TeleportTo(Vector3 position, EmotionType emotion, bool faceRight, SadnessPuzzleRoom roomToActivate)
    {
        StartCoroutine(TeleportRoutine(position, emotion, faceRight, roomToActivate));
    }
    public void TeleportTo(Vector3 position, EmotionType emotion, bool faceRight)
    {
        StartCoroutine(TeleportRoutine(position, emotion, faceRight));
    }

    private IEnumerator TeleportRoutine(Vector3 targetPos, EmotionType emotion, bool faceRight, SadnessPuzzleRoom targetRoom)
    {
        GameStateManager.Instance.SetState(GameState.Loading);

        yield return Fade(1f); // Fade to black

        SadnessPuzzleRoomManager.Instance.EnterRoom(targetRoom);

        // Snap camera to player instantly
        cameraController.SnapToTargetImmediately();

        yield return new WaitForSeconds(0.1f);
        playerTransform.position = targetPos;

        if (playerTransform.TryGetComponent(out PlayerChecks checks))
        {
            checks.SetFacing(faceRight ? Facing.Right : Facing.Left);
        }

        ApplyEmotionVFX(emotion);

        yield return new WaitForSeconds(0.2f);

        yield return Fade(0f); // Fade in

        // Restore smooth camera motion
        cameraController.RestoreCameraDamping();

        GameStateManager.Instance.SetState(GameState.Gameplay);
    }

    private IEnumerator TeleportRoutine(Vector3 targetPos, EmotionType emotion, bool faceRight)
    {
        GameStateManager.Instance.SetState(GameState.Loading);

        yield return Fade(1f); // Fade to black

        // Snap camera to player instantly
        cameraController.SnapToTargetImmediately();

        yield return new WaitForSeconds(0.1f);
        playerTransform.position = targetPos;

        if (playerTransform.TryGetComponent(out PlayerChecks checks))
        {
            checks.SetFacing(faceRight ? Facing.Right : Facing.Left);
        }

        ApplyEmotionVFX(emotion);

        yield return new WaitForSeconds(0.2f);

        yield return Fade(0f); // Fade in

        // Restore smooth camera motion
        cameraController.RestoreCameraDamping();

        GameStateManager.Instance.SetState(GameState.Gameplay);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (screenFade != null)
        {
            screenFade.blocksRaycasts = true;
            yield return screenFade.DOFade(targetAlpha, fadeDuration).WaitForCompletion();
            screenFade.blocksRaycasts = targetAlpha > 0.9f;
        }
    }

    private void ApplyEmotionVFX(EmotionType emotion)
    {
        if (teleportVFX != null && playerTransform != null)
        {
            GameObject fx = Instantiate(teleportVFX, playerTransform.position, Quaternion.identity, vfxSpawnParent);
            Destroy(fx, 2f);
        }

        if (teleportSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(teleportSound);
        }
    }
}
