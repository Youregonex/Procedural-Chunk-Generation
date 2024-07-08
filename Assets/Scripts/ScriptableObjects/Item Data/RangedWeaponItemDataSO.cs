using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Data/Ranged Weapon Item Data")]
public class RangedWeaponItemDataSO : WeaponItemDataSO
{
    [field: SerializeField] public Projectile ProjectilePrefab { get; private set; }
    [field: SerializeField] public float ProjectileSpeed { get; private set; }
    [field: SerializeField] public float ProjectileRange { get; private set; }
}
