using UnityEngine;

public class PuzzleLightController : MonoBehaviour
{
    [Header("Puzzle Configuration")]
    [SerializeField] private Color[] colorCycle;
    [SerializeField] private int[] solutionIndices;

    [Header("References")]
    [SerializeField] private PuzzleLightNode[] lightNodes;
    [SerializeField] private GameObject[] activatableTargets;
    [SerializeField] private bool activateOnlyOnce = true;

    private bool isSolved = false;

    public int ColorCycleLength => colorCycle.Length;
    public Color GetColor(int index) => colorCycle[index % ColorCycleLength];

    public System.Action OnPuzzleSolved;

    private void Awake()
    {
        if (lightNodes == null || lightNodes.Length == 0)
            lightNodes = GetComponentsInChildren<PuzzleLightNode>();

        for (int i = 0; i < lightNodes.Length; i++)
            lightNodes[i].Initialize(i, this);
    }

    private void Start()
    {
        foreach (var node in lightNodes)
        {
            node.SetColor(0);
        }
    }

    public void TrySolvePuzzle()
    {
        if (isSolved) return;

        if (IsPuzzleSolved())
        {
            isSolved = true;
            Debug.Log("Puzzle solved!");

            foreach (var obj in activatableTargets)
            {
                if (obj == null) continue;

                if (obj.TryGetComponent<IActivatable>(out var activatable))
                    activatable.Activate();
            }

            foreach (var node in lightNodes)
                node.SetColor(Color.white);

            OnPuzzleSolved?.Invoke();
        }
        else
        {
            Debug.Log("Puzzle not solved yet.");
        }
    }

    private bool IsPuzzleSolved()
    {
        if (lightNodes.Length != solutionIndices.Length)
        {
            Debug.LogError("Mismatch: Light count and solution length differ!");
            return false;
        }

        for (int i = 0; i < lightNodes.Length; i++)
        {
            if (lightNodes[i].CurrentColorIndex != solutionIndices[i])
                return false;
        }

        return true;
    }
}