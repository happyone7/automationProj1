using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public int x, y;
    public Building building;
    public ItemSlot outputBuffer;  //建筑业에서 나오는 아이템 저장

    public Tile(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.building = null;
        this.outputBuffer = new ItemSlot();
    }

    public bool IsEmpty => building == null;
    public bool HasOutput => !outputBuffer.IsEmpty;
}

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Grid Settings")]
    public int width = 50;
    public int height = 50;
    public float cellSize = 1f;

    private Tile[,] grid;
    private Dictionary<Vector2Int, Tile> tileMap;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        grid = new Tile[width, height];
        tileMap = new Dictionary<Vector2Int, Tile>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = new Tile(x, y);
                grid[x, y] = tile;
                tileMap[new Vector2Int(x, y)] = tile;
            }
        }
    }

    // 타일 가져오기
    public Tile GetTile(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return null;
        return grid[x, y];
    }

    public Tile GetTile(Vector2Int pos) => GetTile(pos.x, pos.y);

    // 배치 가능 여부 확인
    public bool CanPlace(int x, int y, int buildingWidth, int buildingHeight)
    {
        for (int bx = 0; bx < buildingWidth; bx++)
        {
            for (int by = 0; by < buildingHeight; by++)
            {
                int checkX = x + bx;
                int checkY = y + by;

                if (checkX < 0 || checkX >= width || checkY < 0 || checkY >= height)
                    return false;

                if (!grid[checkX, checkY].IsEmpty)
                    return false;
            }
        }
        return true;
    }

    //建筑业 배치
    public bool PlaceBuilding(int x, int y, Building building)
    {
        if (!CanPlace(x, y, building.Data.width, building.Data.height))
            return false;

        for (int bx = 0; bx < building.Data.width; bx++)
        {
            for (int by = 0; by < building.Data.height; by++)
            {
                Tile tile = grid[x + bx, y + by];
                if (bx == 0 && by == 0)
                {
                    tile.building = building;
                    building.SetTile(tile);
                }
                else
                {
                    tile.building = building;  // 같은建筑业 참조
                }
            }
        }

        building.Initialize(x, y);
        return true;
    }

    //建筑业 제거
    public void RemoveBuilding(Building building)
    {
        if (building.Tile == null) return;

        int startX = building.Tile.x;
        int startY = building.Tile.y;

        for (int bx = 0; bx < building.Data.width; bx++)
        {
            for (int by = 0; by < building.Data.height; by++)
            {
                Tile tile = grid[startX + bx, startY + by];
                tile.building = null;
            }
        }

        building.OnRemoved();
    }

    // World Position → Grid Position
    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / cellSize);
        int y = Mathf.FloorToInt(worldPos.y / cellSize);
        return new Vector2Int(x, y);
    }

    // Grid Position → World Position (중앙)
    public Vector3 GridToWorld(int x, int y)
    {
        return new Vector3(
            x * cellSize + cellSize / 2f,
            y * cellSize + cellSize / 2f,
            0
        );
    }

    // 이웃 타일 가져오기
    public Tile GetNeighbor(Tile tile, Direction direction)
    {
        Vector2Int offset = direction.ToVector2Int();
        return GetTile(tile.x + offset.x, tile.y + offset.y);
    }

    // 모든 타일 순회 (외부에서 사용)
    public IEnumerable<Tile> GetAllTiles()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                yield return grid[x, y];
            }
        }
    }

    // Debug Gizmos
    void OnDrawGizmos()
    {
        if (grid == null) return;

        Gizmos.color = Color.gray;
        for (int x = 0; x <= width; x++)
        {
            for (int y = 0; y <= height; y++)
            {
                Vector3 pos = new Vector3(x * cellSize, y * cellSize, 0);
                Gizmos.DrawLine(pos, pos + new Vector3(cellSize, 0, 0));
                Gizmos.DrawLine(pos, pos + new Vector3(0, cellSize, 0));
            }
        }
    }
}
