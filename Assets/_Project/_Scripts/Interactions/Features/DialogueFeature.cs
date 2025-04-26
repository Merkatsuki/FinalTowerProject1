using UnityEngine;

public class DialogueFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Dialogue Settings")]
    [SerializeField] private string oneLiner;
    [SerializeField] private DialogueSequence dialogueSequence;

    public void OnInteract(IPuzzleInteractor actor)
    {
        TriggerDialogue();
    }

    public void TriggerDialogue()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("[DialogueFeature] No DialogueManager found.");
            return;
        }

        if (dialogueSequence != null && dialogueSequence.lines != null && dialogueSequence.lines.Count > 0)
        {
            DialogueManager.Instance.ShowDialogue(dialogueSequence);
        }
        else if (!string.IsNullOrEmpty(oneLiner))
        {
            DialogueManager.Instance.ShowMessage(oneLiner);
        }
        else
        {
            Debug.LogWarning("[DialogueFeature] No dialogue set.");
        }
    }

    public bool HasDialogue()
    {
        return dialogueSequence != null || !string.IsNullOrEmpty(oneLiner);
    }
}
