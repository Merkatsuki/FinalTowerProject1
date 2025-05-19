using UnityEngine;

[CreateAssetMenu(menuName = "Puzzle/Pipe Type", fileName = "NewPipeType")]
public class PipeTypeSO : ScriptableObject
{
    public GameObject pipePrefab;

    [Header("Puzzle Role")]
    public bool isSource = false;
    public bool isReceiver = false;
    public bool isMovable = true;
    public bool isEmpty = false;

    [Header("Open Directions")]
    public bool openTop;
    public bool openBottom;
    public bool openLeft;
    public bool openRight;

    [Header("Rotation")]
    public bool canBeRotated = false;
    public int maxRotationStates = 1; // 1 = fixed, 2 = 180°, 4 = 90° increments, etc.
}
