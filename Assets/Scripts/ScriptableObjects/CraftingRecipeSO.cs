using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crafting Recipe")]
public class CraftingRecipeSO : ScriptableObject
{
    [field: SerializeField] public List<CraftingComponent> RecipeComponents { get; private set; }
    [field: SerializeField] public ItemDataSO RecipeResult { get; private set; }

}
