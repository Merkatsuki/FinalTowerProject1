using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueGraph", menuName = "Dialogue/Dialogue Graph")]
public class DialogueGraphSO : ScriptableObject
{
    public string graphName;
    public string startNodeId;
    public List<DialogueNode> nodes = new();

    // Helper to find node by ID
    public DialogueNode GetNodeById(string id)
    {
        return nodes.Find(node => node.id == id);
    }
}
