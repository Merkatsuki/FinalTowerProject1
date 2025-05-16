using Momentum;
using UnityEngine;

public class CompanionCommandManager : MonoBehaviour
{
    public static CompanionCommandManager Instance { get; private set; }

    [SerializeField] private CompanionController companion;

    private bool isWaitHereToggled = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public bool CanEnterCommandMode()
    {
        return companion != null && !companion.IsInteractionLocked;
    }

    public void EnterCommandMode()
    {
        QuipManager.Instance?.TryPlayCommandStartQuip(companion);
        Debug.Log("[CommandManager] Entered Command Mode");
    }

    public void ExitCommandMode()
    {
        QuipManager.Instance?.TryPlayCommandEndQuip(companion);
        if (isWaitHereToggled)
        {
            QuipManager.Instance?.TryPlayWaitHereQuip(companion);
            companion.CommandWaitHere();
        }
        else
        {
            QuipManager.Instance?.TryPlayFollowResumeQuip(companion);
            companion.fsm.ChangeState(companion.followState);
        }
    }

    public void ToggleWaitHereMode(bool active)
    {
        isWaitHereToggled = active;
        Debug.Log("[CommandManager] Wait Here mode set to: " + isWaitHereToggled);
    }

    public void IssueMoveToPoint(Vector2 worldPosition)
    {
        if (!InputManager.instance.IsCommandMode) return;
        QuipManager.Instance?.TryPlayCommandMoveQuip(companion);
        companion.CommandMoveToPoint(worldPosition);
    }

    public void IssueResumeFollow()
    {
        if (!InputManager.instance.IsCommandMode) return;
        companion.CommandResume();
    }

    public void TryIssueCommand(IWorldInteractable target)
    {
        if (companion.Perception.CanInteractWith(target))

            if (target != null && target.CanBeInteractedWith(companion))
        {
            QuipManager.Instance?.TryPlayCommandInteractQuip(companion);
            companion.IssuePlayerCommand(target);
        }
        else
        {
            QuipManager.Instance?.TryPlayEmotionMismatchQuip(target);
        }
    }

    public bool CanInteractWith(IWorldInteractable target)
    {
        return target != null && target.CanBeInteractedWith(companion);
    }

    public bool IsInCommandMode() => InputManager.instance?.IsCommandMode ?? false;

    public bool IsWaitHereToggled() => isWaitHereToggled;
}
