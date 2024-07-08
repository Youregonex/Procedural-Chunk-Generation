using UnityEngine;

public abstract class WeaponItemDataSO : ItemDataSO
{
    [field: SerializeField] public Weapon WeaponPrefab { get; private set; }
    [field: SerializeField] public float AttackDamageMin { get; private set; }
    [field: SerializeField] public float AttackDamageMax { get; private set; }
    [field: SerializeField] public float KnockbackForce { get; private set; }
    [field: SerializeField] public float AttackCooldown { get; private set; }
}
