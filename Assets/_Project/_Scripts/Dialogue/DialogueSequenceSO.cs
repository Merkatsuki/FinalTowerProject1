using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Sequence", menuName = "Dialogue/Sequence")]
public class DialogueSequenceSO : ScriptableObject
{
    public List<DialogueLine> lines = new();
}