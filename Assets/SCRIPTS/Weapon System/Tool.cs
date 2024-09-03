using UnityEngine;

public class Tool : MeleeWeapon
{
    [Header("Debug Fields")]
    [SerializeField] private EToolType _toolType;
    [SerializeField] private int _ticksPerAttackMin;
    [SerializeField] private int _ticksPerAttackMax;
    [SerializeField] private int _toolTier;


    public override void Attack()
    {
        if (_attackCooldownCurrent > 0)
            return;

        _weaponAnimation.PlayWeaponAttackAnimation();

        Collider2D[] targetsHit = Physics2D.OverlapCircleAll(_attackOrigin.position, _attackRadius);

        foreach (Collider2D hit in targetsHit)
        {
            if (!hit.TryGetComponent(out IDamageable damageable))
                continue;

            if (ReferenceEquals(damageable, _agentCore.GetAgentComponent<AgentHitbox>()))
                continue;

            damageable.TakeDamage(new DamageStruct
            {
                damageSender = gameObject,
                damageAmount = UnityEngine.Random.Range(_attackDamageMin, _attackDamageMax),
                knockbackForce = _knockbackForce
            });
        }

        targetsHit = Physics2D.OverlapCircleAll(_attackOrigin.position, _attackRadius);

        foreach (Collider2D hit in targetsHit)
        {
            IGatherable gatherable = hit.GetComponent<IGatherable>();

            if (gatherable == null)
                continue;

            gatherable.Gather(new GatherStruct
            {
                toolType = _toolType,
                toolTier = _toolTier,
                ticksPerHit = UnityEngine.Random.Range(_ticksPerAttackMin, _ticksPerAttackMax + 1)
            });
        }

        _attackCooldownCurrent = _attackCooldownMax;
    }

    protected override void SetupWeaponStats()
    {
        base.SetupWeaponStats();

        ToolItemDataSO toolItemDataSO = _weaponItemDataSO as ToolItemDataSO;

        _toolType = toolItemDataSO.ToolType;
        _ticksPerAttackMin = toolItemDataSO.TicksPerHitMin;
        _ticksPerAttackMax = toolItemDataSO.TicksPerHitMax;
        _toolTier = toolItemDataSO.ToolTier;
    }
}
