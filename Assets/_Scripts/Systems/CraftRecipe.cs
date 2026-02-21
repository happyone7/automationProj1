using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Automation/Recipe")]
public class CraftRecipe : ScriptableObject
{
    [Header("Recipe Info")]
    public string id;
    public Item output;
    public int outputAmount = 1;
    public float craftTime = 1f;

    [Header("Ingredients")]
    public Ingredient[] inputs;

    [System.Serializable]
    public struct Ingredient
    {
        public Item item;
        public int amount;
    }
}
