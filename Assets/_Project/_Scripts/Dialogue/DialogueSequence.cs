using System.Collections.Generic;

[System.Serializable]
public class DialogueSequence
{
    public string speaker;
    public List<string> lines;
    public bool pauseAfter; // true if the object should wait before exit
}
