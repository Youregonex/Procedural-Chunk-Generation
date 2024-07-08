using UnityEngine;

public class MeleeWeapon : Weapon
{
    [Header("Debug Fields")]
    [SerializeField] private float _attackRadius;


    public override void Attack()
    {
        if (_attackCooldownCurrent > 0)
            return;

        _weaponAnimation.PlayWeaponAttackAnimation();

        Collider2D[] targetsHit = Physics2D.OverlapCircleAll(_attackOrigin.position, _attackRadius);

        foreach (Collider2D hit in targetsHit)
        {
            IDamegeable damageable = hit.GetComponent<IDamegeable>();

            if (damageable == null)
                continue;

            if (damageable == _agentAttackModule.GetComponent<AgentHealthSystem>().GetHitbox())
                continue;

            damageable.TakeDamage(new DamageStruct
            {
                damageSender = gameObject,
                damageAmount = Random.Range(_attackDamageMin, _attackDamageMax),
                knockbackForce = _knockbackForce
            });
        }

        _attackCooldownCurrent = _attackCooldownMax;
    }

    protected override void SetupWeaponStats()
    {
        base.SetupWeaponStats();

        MeleeWeaponItemDataSO meleeWeaponItemDataSO = _weaponItemDataSO as MeleeWeaponItemDataSO;
        _attackRadius = meleeWeaponItemDataSO.AttackRadius;
    }

    protected void OnDrawGizmos()
    {
        if (!_showGizmos)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_attackOrigin.transform.position, _attackRadius);
    }
}
