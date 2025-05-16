// Refactored QuipManager.cs for modular trigger support
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuipManager : MonoBehaviour
{
    public static QuipManager Instance { get; private set; }

    [SerializeField] private CompanionQuipUI companionQuipUI;
    [SerializeField] private float ambientQuipInterval = 45f;
    [SerializeField] private float globalQuipCooldown = 5f;

    [Header("All Quips")]
    public List<RobotQuip> allQuips = new();

    private Dictionary<RobotQuip, int> usageCounts = new();
    private List<RobotQuip> recentQuipHistory = new();
    private const int MAX_HISTORY = 20;

    public EmotionTag currentEmotion = EmotionTag.Neutral;
    public ZoneTag currentZone = ZoneTag.Any;

    private float ambientTimer;
    private float quipCooldownTimer = 0f;
    private bool quipInProgress = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Update()
    {
        ambientTimer += Time.deltaTime;
        quipCooldownTimer += Time.deltaTime;

        if (ambientTimer >= ambientQuipInterval)
        {
            TryPlayAmbientQuip();
            ambientTimer = 0f;
        }
    }

    // Public entry points by trigger type
    public void TryPlayCommandStartQuip(CompanionController context) => TryPlayFilteredQuip(QuipTriggerType.OnCommandStart, context);
    public void TryPlayCommandEndQuip(CompanionController context) => TryPlayFilteredQuip(QuipTriggerType.OnCommandEnd, context);
    public void TryPlayCommandMoveQuip(CompanionController context) => TryPlayFilteredQuip(QuipTriggerType.OnCommandMove, context);
    public void TryPlayCommandInteractQuip(CompanionController context) => TryPlayFilteredQuip(QuipTriggerType.OnCommandInteract, context);
    public void TryPlayWaitHereQuip(CompanionController context) => TryPlayFilteredQuip(QuipTriggerType.OnWaitHere, context);
    public void TryPlayFollowResumeQuip(CompanionController context) => TryPlayFilteredQuip(QuipTriggerType.OnFollowResume, context);
    public void TryPlayEmotionMismatchQuip(IWorldInteractable target)
    {
        Debug.Log("[Quip] Emotion mismatch on: " + target?.GetDisplayName());
        TryPlayFilteredQuip(QuipTriggerType.OnEmotionMismatch, null);
    }

    public void TryPlayFilteredQuip(QuipTriggerType trigger, CompanionController context)
    {
        if (DialogueManager.Instance.IsDialoguePlaying()) return;
        if (quipInProgress || quipCooldownTimer < globalQuipCooldown) return;

        EmotionTag emotion = context?.GetEmotion() ?? currentEmotion;
        ZoneTag zone = currentZone;

        var candidates = allQuips.Where(q =>
            q.triggerType == trigger &&
            (q.emotionTag == EmotionTag.Any || q.emotionTag == emotion) &&
            (q.zoneTag == ZoneTag.Any || q.zoneTag == zone) &&
            HasRemainingUses(q) &&
            IsEligibleFromHistory(q)
        ).ToList();

        if (candidates.Count == 0) return;

        RobotQuip selected = PickWeightedQuip(candidates);
        if (selected != null)
            PlayQuip(selected);
    }

    public void TryPlayAmbientQuip()
    {
        if (DialogueManager.Instance.IsDialoguePlaying()) return;
        if (quipInProgress || quipCooldownTimer < globalQuipCooldown) return;

        var ambientQuips = allQuips.Where(q =>
            q.triggerType == QuipTriggerType.AmbientRandom &&
            (q.emotionTag == EmotionTag.Any || q.emotionTag == currentEmotion) &&
            (q.zoneTag == ZoneTag.Any || q.zoneTag == currentZone) &&
            HasRemainingUses(q) &&
            IsEligibleFromHistory(q)
        ).ToList();

        if (ambientQuips.Count == 0) return;

        RobotQuip selected = PickWeightedQuip(ambientQuips);
        if (selected != null)
            PlayQuip(selected);
    }

    public void PlayDirectQuip(string line)
    {
        if (DialogueManager.Instance.IsDialoguePlaying()) return;
        if (quipInProgress || quipCooldownTimer < globalQuipCooldown) return;

        if (companionQuipUI != null)
        {
            quipInProgress = true;
            companionQuipUI.ShowQuip(line);
            quipCooldownTimer = 0f;
        }
    }
    private void PlayQuip(RobotQuip selected)
    {
        if (companionQuipUI != null)
        {
            quipInProgress = true;
            companionQuipUI.ShowQuip(selected.quipText);
            RegisterQuipHistory(selected);
            IncrementUsage(selected);
        }
    }

    public void NotifyQuipEnded()
    {
        quipInProgress = false;
        quipCooldownTimer = 0f;
    }

    private RobotQuip PickWeightedQuip(List<RobotQuip> quips)
    {
        float totalWeight = quips.Sum(q => q.weight);
        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var quip in quips)
        {
            cumulative += quip.weight;
            if (roll <= cumulative)
                return quip;
        }

        return null;
    }

    private void RegisterQuipHistory(RobotQuip quip)
    {
        recentQuipHistory.Add(quip);
        if (recentQuipHistory.Count > MAX_HISTORY)
            recentQuipHistory.RemoveAt(0);
    }

    private bool IsEligibleFromHistory(RobotQuip quip)
    {
        if (!quip.repeatable)
            return !recentQuipHistory.Contains(quip);

        int index = recentQuipHistory.LastIndexOf(quip);
        if (index == -1) return true;

        int quipsSince = recentQuipHistory.Count - index - 1;
        return quipsSince >= quip.repeatAfterQuips;
    }

    private bool HasRemainingUses(RobotQuip quip)
    {
        if (quip.useLimit < 0) return true;
        return !usageCounts.ContainsKey(quip) || usageCounts[quip] < quip.useLimit;
    }

    private void IncrementUsage(RobotQuip quip)
    {
        if (!usageCounts.ContainsKey(quip)) usageCounts[quip] = 0;
        usageCounts[quip]++;
    }

    public void SetEmotion(EmotionTag emotion) => currentEmotion = emotion;
    public void SetZone(ZoneTag zone) => currentZone = zone;
}
