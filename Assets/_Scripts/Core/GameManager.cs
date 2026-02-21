using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public GridManager gridManager;
    public BuildingPlacer buildingPlacer;

    [Header("Game Settings")]
    public float tickInterval = 0.1f;  // 게임 틱 간격

    private float tickTimer;

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
        Debug.Log("Automation Game Started!");
    }

    void Update()
    {
        // 게임 �ick -建筑业 업데이트
        tickTimer += Time.deltaTime;
        if (tickTimer >= tickInterval)
        {
            tickTimer = 0f;
            OnGameTick();
        }
    }

    private void OnGameTick()
    {
        if (GridManager.Instance == null) return;

        // 모든 타일의建筑业 업데이트
        foreach (Tile tile in GridManager.Instance.GetAllTiles())
        {
            if (tile.building != null)
            {
                tile.building.OnTick(tickInterval);
            }
        }

        // Conway 시스템 업데이트
        if (ConveyorSystem.Instance != null)
        {
            ConveyorSystem.Instance.OnTick(tickInterval);
        }
    }
}
