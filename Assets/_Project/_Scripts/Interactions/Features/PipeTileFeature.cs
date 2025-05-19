using UnityEngine;

public class PipeTileFeature : FeatureBase
{
    [HideInInspector] public int x, y; // managed by grid, not set manually

    [Header("Tile Data")]
    public PipeTypeSO pipeType;

    [Header("Tile Flags")]
    public bool isEmpty = false; // manually set one to true

    [SerializeField] private Transform visualRoot; // placeholder for animating future visuals
    private GameObject activeVisual;

    private bool openTop, openBottom, openLeft, openRight;


    private void OnEnable()
    {
        if (pipeType != null)
            ApplyPipeType();
    }

    public void SetPipeType(PipeTypeSO newType)
    {
        pipeType = newType;
        isEmpty = newType != null && newType.isEmpty;
        ApplyPipeType();
    }

    public void ApplyPipeType()
    {
        // Remove old visual if any
        if (activeVisual != null)
        {
            DestroyImmediate(activeVisual);
        }

        // Instantiate new visual
        if (pipeType.pipePrefab != null && visualRoot != null)
        {
            activeVisual = Instantiate(pipeType.pipePrefab, visualRoot);
            activeVisual.transform.localPosition = Vector3.zero;
        }

        // Set connection logic
        openTop = pipeType.openTop;
        openBottom = pipeType.openBottom;
        openLeft = pipeType.openLeft;
        openRight = pipeType.openRight;
    }

    public bool IsOpen(Direction direction)
    {
        return direction switch
        {
            Direction.Up => openTop,
            Direction.Down => openBottom,
            Direction.Left => openLeft,
            Direction.Right => openRight,
            _ => false
        };
    } 

    public void SetGridPosition(int newX, int newY)
    {
        x = newX;
        y = newY;
    }

    public override void OnInteract(IPuzzleInteractor interactor)
    {
        Debug.Log($"[PipeTileFeature] Interacted at ({x},{y})");
        NotifyPuzzleInteractionSuccess();
        RunFeatureEffects(interactor);
    }

    private void OnMouseDown()
    {
        if (pipeType != null && !pipeType.isMovable) return;

        var manager = FindFirstObjectByType<PipeGridManager>();
        manager?.TrySwapWithEmpty(this);
        Debug.Log($"[PipeTileFeature] Clicked at ({x},{y})");
    }

    public bool IsAdjacentTo(PipeTileFeature other)
    {
        return Mathf.Abs(x - other.x) + Mathf.Abs(y - other.y) == 1;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Vector3 pos = transform.position;

        float len = 0.25f;
        if (openTop) Gizmos.DrawLine(pos, pos + Vector3.up * len);
        if (openBottom) Gizmos.DrawLine(pos, pos + Vector3.down * len);
        if (openLeft) Gizmos.DrawLine(pos, pos + Vector3.left * len);
        if (openRight) Gizmos.DrawLine(pos, pos + Vector3.right * len);
    }
}
