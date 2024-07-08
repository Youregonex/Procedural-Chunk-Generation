using UnityEngine;

public class Tool : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private ToolItemDataSO _toolItemDataSO;
    [SerializeField] private WeaponAnimation _toolAnimation;
    [SerializeField] private Transform _gatherOrigin;

    [Header("Debug Fields")]
    [SerializeField] private float _attackRadius;
    [SerializeField] private float _attackDamageMax;
    [SerializeField] private float _attackDamageMin;
    [SerializeField] private float _knockbackForce;
    [SerializeField] private float _attackCooldownMax;
    [SerializeField] private float _attackCooldownCurrent;
    [SerializeField] private AgentAttackModule _weaponHolder;
    [SerializeField] private bool _showGizmos;
    [SerializeField] private EToolType _toolType;
    [SerializeField] private int _ticksPerAttackMin;
    [SerializeField] private int _ticksPerAttackMax;
    [SerializeField] private int _toolTier;

    public float AttackCooldownCurrent => _attackCooldownCurrent;
    public bool CanSwing => _attackCooldownCurrent <= 0;


    private void Update()
    {
        if (_attackCooldownCurrent > 0)
            _attackCooldownCurrent -= Time.deltaTime;
    }

    public void SetUpTool(AgentAttackModule agentAttackModue)
    {
        _weaponHolder = agentAttackModue;

        SetupToolStats();
    }

    public void Attack()
    {
        if (_attackCooldownCurrent > 0)
            return;

        _toolAnimation.PlayWeaponAttackAnimation();

        Collider2D[] targetsHit = Physics2D.OverlapCircleAll(_gatherOrigin.position, _attackRadius);

        foreach (Collider2D hit in targetsHit)
        {
            IDamegeable damageable = hit.GetComponent<IDamegeable>();

            if (damageable == null)
                continue;

            if (damageable == _weaponHolder.GetComponent<AgentHealthSystem>().GetHitbox())
                continue;

            damageable.TakeDamage(new DamageStruct
            {
                damageSender = gameObject,
                damageAmount = UnityEngine.Random.Range(_attackDamageMin, _attackDamageMax),
                knockbackForce = _knockbackForce
            });
        }

        targetsHit = Physics2D.OverlapCircleAll(_gatherOrigin.position, _attackRadius);

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

    private void SetupToolStats()
    {
        _attackRadius = _toolItemDataSO.AttackRadius;
        _attackDamageMin = _toolItemDataSO.AttackDamageMin;
        _attackDamageMax = _toolItemDataSO.AttackDamageMax;
        _knockbackForce = _toolItemDataSO.KnockbackForce;
        _attackCooldownMax = _toolItemDataSO.AttackCooldown;
        _attackCooldownCurrent = _attackCooldownMax;

        _toolType = _toolItemDataSO.ToolType;
        _ticksPerAttackMin = _toolItemDataSO.TicksPerHitMin;
        _ticksPerAttackMax = _toolItemDataSO.TicksPerHitMax;
        _toolTier = _toolItemDataSO.ToolTier;
    }

    private void OnDrawGizmos()
    {
        if (!_showGizmos)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_gatherOrigin.transform.position, _attackRadius);
    }
}
