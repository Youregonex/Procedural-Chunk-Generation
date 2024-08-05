using System.Collections.Generic;
using UnityEngine;

public static class LootTableGenerator
{
    public static Dictionary<ItemDataSO, int> GetDropTable(DropListDataSO dropListDataSO, bool generateOneFromList = false)
    {
        Dictionary<ItemDataSO, int> dropTable = new Dictionary<ItemDataSO, int>();

        if (generateOneFromList)
        {
            int randomItem = Random.Range(0, dropListDataSO.ItemDropList.Count);

            ItemDropDataStruct itemDrop = dropListDataSO.ItemDropList[randomItem];

            if (Random.Range(0f, 1f) > itemDrop.dropChance)
                return dropTable;

            dropTable.Add(itemDrop.dropResource, Random.Range(itemDrop.dropResourceAmountMin, itemDrop.dropResourceAmountMax + 1));

            return dropTable;
        }

        foreach (ItemDropDataStruct itemDrop in dropListDataSO.ItemDropList)
        {
            if (Random.Range(0f, 1f) > itemDrop.dropChance)
                continue;

            dropTable.Add(itemDrop.dropResource, Random.Range(itemDrop.dropResourceAmountMin, itemDrop.dropResourceAmountMax + 1));
        }

        return dropTable;
    }
}
