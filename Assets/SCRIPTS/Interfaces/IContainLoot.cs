using System.Collections.Generic;
using System;

public interface IContainLoot
{
    public event Action OnLootDrop;
    public void FillLootList(List<Item> lootList);
}
