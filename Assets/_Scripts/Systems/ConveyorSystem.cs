using UnityEngine;

public class ConveyorTile
{
    public Tile tile;
    public ItemSlot itemSlot;        // 현재 위치한 아이템
    public float progress;           // 0~1 사이 이동 진행률
    public Direction movementDir;    // 이동 방향

    public ConveyorTile(Tile tile)
    {
        this.tile = tile;
        this.itemSlot = new ItemSlot();
        this.progress = 0f;
        this.movementDir = Direction.Right;  // 기본값
    }

    public bool HasItem => !itemSlot.IsEmpty;
}

public class ConveyorSystem : MonoBehaviour
{
    public static ConveyorSystem Instance { get; private set; }

    [Header("Settings")]
    public float moveSpeed = 1f;  // 타일당 이동 시간 (초)
    
    // Conway 타일들을 추적
    private ConveyorTile[,] conveyorMap;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        InitializeConveyorMap();
    }

    private void InitializeConveyorMap()
    {
        GridManager grid = GridManager.Instance;
        if (grid == null) return;

        conveyorMap = new ConveyorTile[grid.width, grid.height];
    }

    // Conway 업데이트 (매 게임 틱)
    public void OnTick(float deltaTime)
    {
        GridManager grid = GridManager.Instance;
        if (grid == null) return;

        // Conway 타일 순회 - 뒤에서부터 (이동先把 뒤에꺼 처리)
        for (int y = 0; y < grid.height; y++)
        {
            for (int x = 0; x < grid.width; x++)
            {
                Tile tile = grid.GetTile(x, y);
                if (tile.building?.Data?.id == "conveyor")
                {
                    ProcessConveyorTile(tile, deltaTime);
                }
            }
        }
    }

    private void ProcessConveyorTile(Tile tile, float deltaTime)
    {
        if (conveyorMap == null) return;

        // Conway 초기화 (처음이면)
        int x = tile.x;
        int y = tile.y;
        
        if (x < 0 || x >= conveyorMap.GetLength(0) || y < 0 || y >= conveyorMap.GetLength(1))
            return;

        if (conveyorMap[x, y] == null)
        {
            conveyorMap[x, y] = new ConveyorTile(tile);
        }

        ConveyorTile conveyor = conveyorMap[x, y];
        
        // 현재建筑业의 출력 방향 가져오기
        if (tile.building != null)
        {
            conveyor.movementDir = tile.building.CurrentDirection;
        }

        // 아이템이 있으면 이동 시도
        if (conveyor.HasItem)
        {
            conveyor.progress += deltaTime / moveSpeed;

            if (conveyor.progress >= 1f)
            {
                // 이동 완료 - 이웃 타일로 전달
                TryMoveToNeighbor(conveyor);
                conveyor.progress = 0f;
            }
        }
        else
        {
            // 빈 Conway면建筑业에서 아이템 가져오기 시도
            TryPullFromBuilding(conveyor);
        }
    }

    private void TryPullFromBuilding(ConveyorTile conveyor)
    {
        Tile tile = conveyor.tile;
        if (tile.building == null) return;

        //建筑业의 출력 슬롯에서 아이템 가져오기
        ItemSlot output = tile.building.outputSlot;
        if (!output.IsEmpty)
        {
            conveyor.itemSlot = output.Clone();
            output.Clear();
        }
    }

    private void TryMoveToNeighbor(ConveyorTile conveyor)
    {
        Tile currentTile = conveyor.tile;
        Tile neighbor = GridManager.Instance.GetNeighbor(currentTile, conveyor.movementDir);

        if (neighbor == null)
        {
            // 타일 밖으로 나가면 아이템 제거 (또는 드롭)
            conveyor.itemSlot.Clear();
            return;
        }

        // 이웃 타일에建筑业이 있으면 전달 시도
        if (neighbor.building != null)
        {
            // Conway면Chain,建筑业이면Input
            if (neighbor.building.Data?.id == "conveyor")
            {
                // Conway로 전달
                ConveyorTile neighborConveyor = GetOrCreateConveyorTile(neighbor);
                if (!neighborConveyor.HasItem)
                {
                    neighborConveyor.itemSlot = conveyor.itemSlot.Clone();
                    conveyor.itemSlot.Clear();
                }
            }
            else
            {
                //建筑业의 입력 슬롯에 전달
                Building building = neighbor.building;
                int leftover = building.inputSlot.Add(conveyor.itemSlot.item, conveyor.itemSlot.amount);
                
                if (leftover == 0)
                {
                    conveyor.itemSlot.Clear();
                }
                else
                {
                    conveyor.itemSlot.amount = leftover;
                }
            }
        }
        else
        {
            // 빈 타일이면 그냥 이동
            ConveyorTile neighborConveyor = GetOrCreateConveyorTile(neighbor);
            if (!neighborConveyor.HasItem)
            {
                neighborConveyor.itemSlot = conveyor.itemSlot.Clone();
                conveyor.itemSlot.Clear();
            }
        }
    }

    private ConveyorTile GetOrCreateConveyorTile(Tile tile)
    {
        int x = tile.x;
        int y = tile.y;

        if (x < 0 || x >= conveyorMap.GetLength(0) || y < 0 || y >= conveyorMap.GetLength(1))
            return null;

        if (conveyorMap[x, y] == null)
        {
            conveyorMap[x, y] = new ConveyorTile(tile);
        }

        return conveyorMap[x, y];
    }
}
