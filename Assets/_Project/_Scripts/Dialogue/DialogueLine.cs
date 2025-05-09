using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    [TextArea(2, 4)]
    public string text;
    public float pauseAfter = 0f;  // Optional pause after line
    public bool waitForInput = true; // Require input to advance
}
