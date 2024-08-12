using UnityEngine;
using System.Collections.Generic;

public class PlayerCraftingSystem : AgentMonoBehaviourComponent
{
    [SerializeField] private List<CraftingRecipeSO> _availableCraftingRecipeList;

    public List<CraftingRecipeSO> GetAvailableCraftingRecipes() => _availableCraftingRecipeList;

    public override void DisableComponent()
    {
        this.enabled = false;
    }

    public override void EnableComponent()
    {
        this.enabled = true;
    }
}
