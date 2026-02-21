using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingPlacer : MonoBehaviour
{
    public static BuildingPlacer Instance { get; private set; }

    [Header("References")]
    public Camera mainCamera;
    public Grid grid;
    public Tilemap groundTilemap;

    [Header("Placement State")]
    public BuildingData selectedBuilding;
    public Direction currentDirection = Direction.Up;
    public bool isPlacing;

    [Header("Preview")]
    public GameObject previewObject;
    public Color validColor = new Color(0, 1, 0, 0.5f);
    public Color invalidColor = new Color(1, 0, 0, 0.5f);

    private SpriteRenderer previewRenderer;

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
        if (previewObject != null)
        {
            previewRenderer = previewObject.GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        if (selectedBuilding == null) return;

        UpdatePreviewPosition();

        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceBuilding();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RotatePreview();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
        }
    }

    //建筑业 선택
    public void SelectBuilding(BuildingData data)
    {
        selectedBuilding = data;
        isPlacing = true;
        currentDirection = Direction.Up;

        if (previewObject != null && data != null)
        {
            previewObject.SetActive(true);
            //建筑业 아이콘을 프리뷰로 사용 (나중에 실제 프리팹으로 변경)
            if (data.icon != null)
            {
                previewRenderer.sprite = data.icon;
            }
        }
    }

    // Preview 위치 업데이트
    private void UpdatePreviewPosition()
    {
        if (previewObject == null || selectedBuilding == null) return;

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // Tilemap의 셀 크기에 맞춤
        Vector3Int cellPos = grid.WorldToCell(mousePos);
        Vector3 snappedPos = grid.GetCellCenterWorld(cellPos);

        // width/height 고려 -建筑业의 중심이 아니라 왼쪽 아래 모서리 기준으로
        int offsetX = selectedBuilding.width / 2;
        int offsetY = selectedBuilding.height / 2;

        previewObject.transform.position = new Vector3(
            snappedPos.x - offsetX + 0.5f,
            snappedPos.y - offsetY + 0.5f,
            0
        );

        // 배치 가능 여부에 따라 색상 변경
        bool canPlace = CanPlaceAt(cellPos.x, cellPos.y);
        previewRenderer.color = canPlace ? validColor : invalidColor;
    }

    // 해당 위치에 배치 가능한지
    private bool CanPlaceAt(int x, int y)
    {
        if (GridManager.Instance == null) return false;
        return GridManager.Instance.CanPlace(x, y, selectedBuilding.width, selectedBuilding.height);
    }

    //建筑业 배치 시도
    private void TryPlaceBuilding()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = grid.WorldToCell(mousePos);

        if (CanPlaceAt(cellPos.x, cellPos.y))
        {
            PlaceBuilding(cellPos.x, cellPos.y);
        }
    }

    //建筑业 실제로 배치
    private void PlaceBuilding(int x, int y)
    {
        if (selectedBuilding.prefab == null)
        {
            Debug.LogWarning($"Building {selectedBuilding.displayName} has no prefab!");
            return;
        }

        GameObject newBuildingObj = Instantiate(selectedBuilding.prefab);
        Building building = newBuildingObj.GetComponent<Building>();

        if (building == null)
        {
            building = newBuildingObj.AddComponent<Building>();
        }

        building.Initialize(selectedBuilding);
        
        // 방향 설정
        for (int i = 0; i < (int)currentDirection; i++)
        {
            building.Rotate();
        }

        if (GridManager.Instance.PlaceBuilding(x, y, building))
        {
            Debug.Log($"Placed {selectedBuilding.displayName} at ({x}, {y})");
        }
        else
        {
            Destroy(newBuildingObj);
            Debug.LogWarning("Failed to place building!");
        }
    }

    // 회전
    private void RotatePreview()
    {
        currentDirection = currentDirection.RotateClockwise();
        
        if (previewObject != null)
        {
            previewObject.transform.Rotate(0, 0, -90);
        }
    }

    // 배치 취소
    public void CancelPlacement()
    {
        selectedBuilding = null;
        isPlacing = false;

        if (previewObject != null)
        {
            previewObject.SetActive(false);
        }
    }
}
