using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

// 샘플 데이터 생성기 - 에디터에서 실행
public class SampleDataGenerator : MonoBehaviour
{
    // 이 클래스는 에디터에서만 사용됨
#if UNITY_EDITOR
    [MenuItem("Automation/Generate Sample Data")]
    public static void GenerateSampleData()
    {
        // Item 생성
        GenerateItems();
        
        // Recipe 생성
        GenerateRecipes();
        
        // BuildingData 생성
        GenerateBuildings();
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("Sample data generated!");
    }

    private static void GenerateItems()
    {
        string[] itemNames = { "Iron Ore", "Copper Ore", "Stone", "Coal", "Iron Ingot", "Copper Ingot", "Gear" };
        string[] itemIds = { "iron_ore", "copper_ore", "stone", "coal", "iron_ingot", "copper_ingot", "gear" };

        for (int i = 0; i < itemNames.Length; i++)
        {
            string path = $"Assets/_Resources/Items/{itemIds[i]}.asset";
            
            // 이미 있으면 건너뛰기
            if (AssetDatabase.LoadAssetAtPath<Item>(path) != null)
                continue;

            Item item = ScriptableObject.CreateInstance<Item>();
            item.id = itemIds[i];
            item.displayName = itemNames[i];
            item.maxStack = 64;

            AssetDatabase.CreateAsset(item, path);
        }
    }

    private static void GenerateRecipes()
    {
        // Iron Ingot: Iron Ore x1 -> Iron Ingot x1
        CreateRecipe("iron_ingot_recipe", "Iron Ingot", "iron_ore", 1, "iron_ingot", 1, 2f);

        // Copper Ingot: Copper Ore x1 -> Copper Ingot x1  
        CreateRecipe("copper_ingot_recipe", "Copper Ingot", "copper_ore", 1, "copper_ingot", 1, 2f);

        // Gear: Iron Ingot x2 -> Gear x1
        CreateRecipe("gear_recipe", "Gear", "iron_ingot", 2, "gear", 1, 3f);
    }

    private static void CreateRecipe(string id, string outputName, string inputId, int inputAmount, string outputId, int outputAmount, float time)
    {
        string path = $"Assets/_Resources/Recipes/{id}.asset";
        
        if (AssetDatabase.LoadAssetAtPath<CraftRecipe>(path) != null)
            return;

        CraftRecipe recipe = ScriptableObject.CreateInstance<CraftRecipe>();
        recipe.id = id;

        Item inputItem = AssetDatabase.LoadAssetAtPath<Item>($"Assets/_Resources/Items/{inputId}.asset");
        Item outputItem = AssetDatabase.LoadAssetAtPath<Item>($"Assets/_Resources/Items/{outputId}.asset");

        if (inputItem != null && outputItem != null)
        {
            recipe.inputs = new CraftRecipe.Ingredient[1];
            recipe.inputs[0] = new CraftRecipe.Ingredient { item = inputItem, amount = inputAmount };
            recipe.output = outputItem;
            recipe.outputAmount = outputAmount;
            recipe.craftTime = time;

            AssetDatabase.CreateAsset(recipe, path);
        }
    }

    private static void GenerateBuildings()
    {
        // Miner (placeholder - 실제 sprite 필요)
        CreateBuilding("miner", "Miner", null, 1, 1, false);

        // Conveyor
        CreateBuilding("conveyor", "Conveyor Belt", null, 1, 1, true);

        // Furnace ( 크래프팅 )
        CreateBuilding("furnace", "Furnace", null, 1, 1, false);
    }

    private static void CreateBuilding(string id, string name, Sprite icon, int width, int height, bool canRotate)
    {
        string path = $"Assets/_Resources/Buildings/{id}.asset";
        
        if (AssetDatabase.LoadAssetAtPath<BuildingData>(path) != null)
            return;

        BuildingData data = ScriptableObject.CreateInstance<BuildingData>();
        data.id = id;
        data.displayName = name;
        data.icon = icon;
        data.width = width;
        data.height = height;
        data.canRotate = canRotate;

        AssetDatabase.CreateAsset(data, path);
    }
#endif
}
