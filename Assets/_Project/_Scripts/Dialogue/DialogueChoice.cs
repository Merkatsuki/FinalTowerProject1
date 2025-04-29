using System;

[Serializable]
public class DialogueChoice
{
    public string text;        // Text shown on choice button
    public string nextNodeId;   // ID of the DialogueNode this choice leads to
}
