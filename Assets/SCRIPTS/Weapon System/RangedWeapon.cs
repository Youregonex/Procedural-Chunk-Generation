using UnityEngine;

public class RangedWeapon : Weapon
{
    [Header("Debug Fields")]
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _projectileRange;

    protected void Awake()
    {
        _weaponAnimation.OnAttackAnimationFinished += WeaponAnimation_OnAttackAnimationFinished;
    }

    private void OnDestroy()
    {
        _weaponAnimation.OnAttackAnimationFinished -= WeaponAnimation_OnAttackAnimationFinished;
    }

    public override void Attack()
    {
        if (_attackCooldownCurrent > 0)
            return;

        _weaponAnimation.PlayWeaponAttackAnimation();

        _attackCooldownCurrent = _attackCooldownMax;
    }

    protected override void SetupWeaponStats()
    {
        base.SetupWeaponStats();

        RangedWeaponItemDataSO rangedWeaponItemDataSO = _weaponItemDataSO as RangedWeaponItemDataSO;

        _projectilePrefab = rangedWeaponItemDataSO.ProjectilePrefab;
        _projectileSpeed = rangedWeaponItemDataSO.ProjectileSpeed;
        _projectileRange = rangedWeaponItemDataSO.ProjectileRange;
    }

    private void WeaponAnimation_OnAttackAnimationFinished()
    {
        SpawnProjectile();
    }

    private void SpawnProjectile()
    {
        Projectile projectile = Instantiate(_projectilePrefab, _attackOrigin.position, _itemHoldPoint.transform.rotation);

        DamageStruct damageStruct = new DamageStruct()
        {
            damageAmount = Random.Range(_attackDamageMin, _attackDamageMax),
            knockbackForce = _knockbackForce,
            damageSender = _agentAttackModule.gameObject,
            senderFaction = _agentCore.GetFaction()
        };

        projectile.SetupProjectile(_projectileSpeed, _projectileRange, damageStruct);
    }
}
