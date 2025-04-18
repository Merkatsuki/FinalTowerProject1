using UnityEngine;

public class PortalInteractable : InteractableBase
{
    [SerializeField] private GameObject highlightVisual;

    [Header("Scene Loading")]
    [SerializeField] private string targetSceneName;

    [Header("Unlocking")]
    [SerializeField] private bool unlockedByDefault = true;
    [SerializeField] private GameObject lockedIcon;



    private bool isUnlocked = false;

    private void Start()
    {
        isUnlocked = unlockedByDefault;

        if (lockedIcon != null)
            lockedIcon.SetActive(!isUnlocked);
    }

    public void Unlock()
    {
        isUnlocked = true;

        if (lockedIcon != null)
            lockedIcon.SetActive(false);
    }

    public override void OnInteract()
    {
        if (!isUnlocked)
        {
            Debug.Log("This portal is currently locked.");
            return;
        }

        if (!string.IsNullOrEmpty(targetSceneName))
        {
            Debug.Log($"Loading scene: {targetSceneName}");
            SceneLoader.Instance?.LoadScene(targetSceneName);
        }
    }

    public override void OnFocusEnter() => SetHighlighted(true);
    public override void OnFocusExit() => SetHighlighted(false);

    public override void SetHighlighted(bool isHighlighted)
    {
        if (highlightVisual != null)
            highlightVisual.SetActive(isHighlighted);
    }
}
