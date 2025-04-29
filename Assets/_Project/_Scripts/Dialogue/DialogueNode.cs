using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueNode
{
    public string id;                  // Unique ID for this node
    public string speakerName;          // Who is speaking
    [TextArea(3, 6)]
    public string lineText;             // What they say
    public List<DialogueChoice> choices = new(); // Branching choices

    // Optional: allow auto-advance if no choices
    public bool autoAdvance = true;
}
