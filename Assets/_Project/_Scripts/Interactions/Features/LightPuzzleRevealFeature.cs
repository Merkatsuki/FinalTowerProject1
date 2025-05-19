using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LightPuzzleRevealFeature : FeatureBase
{
    [Header("Linked Systems")]
    [SerializeField] private ElevatorPlatformFeature elevator;

    [Header("Puzzle Canvas Fade Settings")]
    [SerializeField] private CanvasGroup puzzleRevealCanvas;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float fadeInDuration = 3f;
    [SerializeField] private float shownAlpha = 1f;
    [SerializeField] private float hiddenAlpha = 0f;

    private Tween activeFadeTween;
    private bool isRevealed = false;

    private void Start()
    {
        // Ensure it's initially active and blocking
        if (puzzleRevealCanvas != null)
        {
            puzzleRevealCanvas.alpha = 1f;
            puzzleRevealCanvas.interactable = true;
            puzzleRevealCanvas.blocksRaycasts = true;
        }
    }

    public override void OnInteract(IPuzzleInteractor actor)
    {
        // Toggle puzzle state
        isRevealed = !isRevealed;

        if (elevator != null)
        {
            var playerInteractor = ReferenceManager.Instance.Player.GetComponentInChildren<PlayerInteractor>();
            elevator.SetAllowControl(!isRevealed); // Lock when canvas is on
            playerInteractor.PuzzleModeOn = isRevealed;
            Debug.Log($"[LightPuzzleReveal] Puzzle {(isRevealed ? "revealed, elevator locked" : "hidden, elevator unlocked")}.");
        }

        ToggleCanvas(isRevealed);
        RunFeatureEffects(actor); // Chain to light toggle etc.
    }

    private void ToggleCanvas(bool fadeOut)
    {
        if (puzzleRevealCanvas == null)
        {
            Debug.LogWarning("[LightPuzzleReveal] No CanvasGroup assigned.");
            return;
        }

        if (activeFadeTween != null && activeFadeTween.IsActive())
            activeFadeTween.Kill();

        float targetAlpha = fadeOut ? hiddenAlpha : shownAlpha;
        float duration = fadeOut ? fadeOutDuration : fadeInDuration;
        bool interactivity = !fadeOut;

        activeFadeTween = DOTween.To(
            () => puzzleRevealCanvas.alpha,
            x => puzzleRevealCanvas.alpha = x,
            targetAlpha,
            duration
        ).OnComplete(() =>
        {
            puzzleRevealCanvas.interactable = interactivity;
            puzzleRevealCanvas.blocksRaycasts = interactivity;
            Debug.Log($"[LightPuzzleReveal] Canvas {(fadeOut ? "faded out" : "faded in")} in {duration}s.");
        });
    }
}