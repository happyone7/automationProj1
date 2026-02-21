using UnityEngine;

[CreateAssetMenu(fileName = "NewBuilding", menuName = "Automation/Building")]
public class BuildingData : ScriptableObject
{
    [Header("Basic Info")]
    public string id;
    public string displayName;
    public Sprite icon;
    public GameObject prefab;

    [Header("Size")]
    public int width = 1;
    public int height = 1;
    public bool canRotate = true;

    [Header("IO - Conveyor Connection")]
    public Direction inputDirection = Direction.Down;
    public Direction outputDirection = Direction.Up;

    [Header("Crafting (null = no crafting)")]
    public CraftRecipe recipe;
    public float processTime = 1f;

    [Header("Miner (null = not a miner)")]
    public Item miningOutput;      // 채굴로 얻는 아이템
    public float miningInterval = 2f;  // 채굴 간격
}
