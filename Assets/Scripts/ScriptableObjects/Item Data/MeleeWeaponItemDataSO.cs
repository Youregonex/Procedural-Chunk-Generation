using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Data/Melee Weapon Item Data")]
public class MeleeWeaponItemDataSO : WeaponItemDataSO
{
    [field: SerializeField] public float AttackRadius { get; private set; }
}
