using UnityEngine;
using System.Collections.Generic;

public class CraftingSystem : MonoBehaviour
{
    public static CraftingSystem Instance { get; private set; }

    [Header("Recipe Database")]
    public List<CraftRecipe> recipes = new List<CraftRecipe>();

    // 인벤토리 참조 (나중에 PlayerInventory로 변경)
    public Inventory targetInventory;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // 레시피 검색
    public CraftRecipe GetRecipe(string recipeId)
    {
        return recipes.Find(r => r.id == recipeId);
    }

    // 만들 수 있는 레시피 목록
    public List<CraftRecipe> GetAvailableRecipes()
    {
        List<CraftRecipe> available = new List<CraftRecipe>();
        
        foreach (var recipe in recipes)
        {
            if (CanCraft(recipe))
            {
                available.Add(recipe);
            }
        }
        
        return available;
    }

    // 만들 수 있는지 확인
    public bool CanCraft(CraftRecipe recipe)
    {
        if (recipe == null || targetInventory == null) return false;

        // 각 재료가 충분한지 확인
        foreach (var ingredient in recipe.inputs)
        {
            int have = targetInventory.GetItemCount(ingredient.item.id);
            if (have < ingredient.amount)
            {
                return false;
            }
        }
        
        return true;
    }

    // 크래프팅 실행
    public bool Craft(CraftRecipe recipe)
    {
        if (!CanCraft(recipe)) return false;

        // 재료 소모
        foreach (var ingredient in recipe.inputs)
        {
            targetInventory.RemoveItem(ingredient.item.id, ingredient.amount);
        }

        // 결과물 추가
        int leftover = targetInventory.AddItem(recipe.output, recipe.outputAmount);
        
        if (leftover > 0)
        {
            //TODO: 인벤토리가 가득 찼을 때 처리
            Debug.LogWarning($"Inventory full! Could not add {leftover} items.");
        }

        return true;
    }

    // 레시피 추가 (에디터용)
    public void AddRecipe(CraftRecipe recipe)
    {
        if (!recipes.Contains(recipe))
        {
            recipes.Add(recipe);
        }
    }

    // 레시피 제거 (에디터용)
    public void RemoveRecipe(CraftRecipe recipe)
    {
        recipes.Remove(recipe);
    }
}
