public interface IDialogueProvider
{
    string GetDialogueLine(); // One-liner fallback
    DialogueSequence GetDialogueSequence(); // Optional full sequence
}
