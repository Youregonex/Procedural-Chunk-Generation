using UnityEngine;

public class WeaponFactory
{
    public Weapon CreateWeapon(WeaponItemDataSO weaponItemDataSO)
    {
        Weapon weapon = GameObject.Instantiate(weaponItemDataSO.WeaponPrefab);

        return weapon;
    }

    public Tool CreateTool(ToolItemDataSO toolItemDataSO)
    {
        Tool tool = GameObject.Instantiate(toolItemDataSO.ToolPrefab);

        return tool;
    }
}
