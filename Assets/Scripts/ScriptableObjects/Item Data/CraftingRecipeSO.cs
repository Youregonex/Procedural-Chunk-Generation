using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Data/Crafting Recipe")]
public class CraftingRecipeSO : ScriptableObject
{
    [field: SerializeField] public List<CraftingComponentStruct> RecipeComponentsList { get; private set; }
    [field: SerializeField] public ItemDataSO RecipeResult { get; private set; }

}
