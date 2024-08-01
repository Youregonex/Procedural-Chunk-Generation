using UnityEngine;
using System.Collections.Generic;

public class PlayerCraftingSystem : MonoBehaviour
{
    [SerializeField] private List<CraftingRecipeSO> _availableCraftingRecipeList;

    public List<CraftingRecipeSO> GetAvailableCraftingRecipes() => _availableCraftingRecipeList;
}
