using DG.Tweening;
using System.Collections;
using UnityEngine;

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

    public void TeleportTo(Vector3 targetPos, EmotionType emotion)
    {
        StartCoroutine(TeleportRoutine(targetPos, emotion));
    }

    private IEnumerator TeleportRoutine(Vector3 targetPos, EmotionType emotion)
    {
        GameStateManager.Instance.SetState(GameState.Loading);

        yield return Fade(1f); // Fade to black

        // Snap camera to player instantly
        cameraController.SnapToTargetImmediately();

        yield return new WaitForSeconds(0.1f);
        playerTransform.position = targetPos;
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
