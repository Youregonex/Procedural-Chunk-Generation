using UnityEngine;

[System.Serializable]
public class ItemSaveData : SaveData
{
    public Vector2 position;
    public ItemDataSO itemDataSO;
    public int quantity;

    public ItemSaveData(Vector2 position, ItemDataSO itemDataSO, int quantity)
    {
        this.position = position;
        this.itemDataSO = itemDataSO;
        this.quantity = quantity;
    }
}
