using UnityEngine;
using System.Collections.Generic;

public class PlayerCraftingSystem : AgentNetworkBehaviourComponent
{
    [SerializeField] private List<CraftingRecipeSO> _availableCraftingRecipeList;

    public List<CraftingRecipeSO> GetAvailableCraftingRecipes() => _availableCraftingRecipeList;

    public override void Initialize() {}

    public override void DisableComponent()
    {
        this.enabled = false;
    }

    public override void EnableComponent()
    {
        this.enabled = true;
    }
}
