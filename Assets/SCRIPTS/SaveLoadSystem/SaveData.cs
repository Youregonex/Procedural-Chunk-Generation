using System.Collections.Generic;

public class SaveData
{
    private List<Item> _itemsOnGround;
    private List<Building> _buildings;

    public SaveData()
    {

    }

    public SaveData(List<Item> itemsOnGround, List<Building> buildings)
    {
        _itemsOnGround = itemsOnGround;
        _buildings = buildings;
    }
}
