using UnityEngine;

[System.Serializable]
public class PipeTileRow
{
    public PipeTileFeature[] tiles;
}

public class PipeGridManager : MonoBehaviour
{
    [Header("Grid Dimensions (for debug only)")]
    public int width = 5;
    public int height = 5;

    [Header("Grid Layout")]
    public PipeTileRow[] rows; // assigned manually in the Inspector

    private PipeTileFeature[,] grid; // built at runtime — do not expose

    private void Start()
    {
        BuildRuntimeGrid();
        RunLightPropagation();
    }

    private void BuildRuntimeGrid()
    {
        grid = new PipeTileFeature[width, height];

        for (int y = 0; y < height; y++)
        {
            var row = rows[y];
            for (int x = 0; x < width; x++)
            {
                var tile = row.tiles[x];
                if (tile != null)
                {
                    grid[x, y] = tile;
                    tile.SetGridPosition(x, y);
                }
            }
        }
    }

    public void TrySwapWithEmpty(PipeTileFeature clicked)
    {
        var empty = FindAdjacentEmptyTile(clicked);
        if (empty == null) return;

        // Swap PipeTypeSOs and apply them to update visuals + logic
        PipeTypeSO tempType = clicked.pipeType;
        clicked.SetPipeType(empty.pipeType);
        empty.SetPipeType(tempType);

        // No need to move transforms or swap in grid
        RunLightPropagation();
    }


    public PipeTileFeature FindAdjacentEmptyTile(PipeTileFeature tile)
    {
        Vector2Int[] directions = {
        new Vector2Int(0, 1), new Vector2Int(0, -1),
        new Vector2Int(1, 0), new Vector2Int(-1, 0)
    };

        foreach (var dir in directions)
        {
            int checkX = tile.x + dir.x;
            int checkY = tile.y + dir.y;

            if (checkX < 0 || checkY < 0 || checkX >= width || checkY >= height) continue;

            var neighbor = grid[checkX, checkY];
            if (neighbor != null && neighbor.isEmpty)
            {
                return neighbor;
            }
        }

        return null;
    }

    public void RunLightPropagation()
    {
        foreach (var tile in grid)
        {
            if (tile == null) continue;
            tile.GetComponent<LightReceiverFeature>()?.SetPowerLogicState(false);
        }

        foreach (var tile in grid)
        {
            if (tile == null || tile.pipeType == null) continue;
            if (tile.pipeType.isSource)
            {
                PropagateFrom(tile);
            }
        }

        foreach (var tile in grid)
        {
            if (tile == null) continue;
            tile.GetComponent<LightReceiverFeature>()?.FinalizePowerState();
        }
    }

    private void PropagateFrom(PipeTileFeature tile)
    {
        var light = tile.GetComponent<LightReceiverFeature>();
        if (light == null || light.IsPowered()) return;

        light?.SetPowerLogicState(true);

        // Check each open direction
        for (int i = 0; i < 4; i++)
        {
            Direction dir = (Direction)i;

            if (!tile.IsOpen(dir)) continue;

            Vector2Int neighborPos = new Vector2Int(tile.x, tile.y) + DirectionToOffset(dir);
            if (!IsInsideGrid(neighborPos)) continue;

            var neighbor = grid[neighborPos.x, neighborPos.y];
            if (neighbor == null || !neighbor.IsOpen(Opposite(dir))) continue;

            PropagateFrom(neighbor); // Recursively light connected neighbor
        }
    }

    private bool IsInsideGrid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < width && pos.y < height;
    }

    private Direction Opposite(Direction dir)
    {
        return dir switch
        {
            Direction.Up => Direction.Down,
            Direction.Down => Direction.Up,
            Direction.Left => Direction.Right,
            Direction.Right => Direction.Left,
            _ => dir
        };
    }

    private Vector2Int DirectionToOffset(Direction dir)
    {
        return dir switch
        {
            Direction.Up => new Vector2Int(0, -1),
            Direction.Down => new Vector2Int(0, 1),
            Direction.Left => new Vector2Int(-1, 0),
            Direction.Right => new Vector2Int(1, 0),
            _ => Vector2Int.zero
        };

    }
}


public enum Direction
{
    Up,
    Down,
    Left,
    Right
}