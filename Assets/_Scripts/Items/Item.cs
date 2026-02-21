using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Automation/Item")]
public class Item : ScriptableObject
{
    [Header("Basic Info")]
    public string id;
    public string displayName;
    public Sprite icon;
    
    [Header("Properties")]
    public int maxStack = 64;
    public bool isBuilding;
}
