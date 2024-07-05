using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private WeaponItemDataSO _weaponItemDataSO;
    [SerializeField] private WeaponAnimation _weaponAnimation;
    [SerializeField] private Transform _attackOrigin;

    [Header("Debug Fields")]
    [SerializeField] private float _attackRadius;
    [SerializeField] private float _attackDamageMax;
    [SerializeField] private float _attackDamageMin;
    [SerializeField] private float _knockbackForce;
    [SerializeField] private float _attackCooldownMax;
    [SerializeField] private float _attackCooldownCurrent = 0f;
    [SerializeField] private AgentAttackModule _weaponHolder;
    [SerializeField] private bool _showGizmos;

    public bool CanSwing => _attackCooldownCurrent <= 0;
    public float AttackCooldownCurrent => _attackCooldownCurrent;

    private void Awake()
    {
        SetupWeapon();
    }

    private void Update()
    {
        if(_attackCooldownCurrent > 0)
            _attackCooldownCurrent -= Time.deltaTime;
    }

    public void SetupWeapon()
    {
        _weaponHolder = transform.root.GetComponent<AgentAttackModule>();

        SetupWeaponStats();
    }

    public void Attack()
    {
        if (_attackCooldownCurrent > 0)
            return;

        _weaponAnimation.PlayWeaponAttackAnimation();

        Collider2D[] targetsHit = Physics2D.OverlapCircleAll(_attackOrigin.position, _attackRadius);

        foreach(Collider2D hit in targetsHit)
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

        _attackCooldownCurrent = _attackCooldownMax;
    }

    private void SetupWeaponStats()
    {
        _attackRadius = _weaponItemDataSO.AttackRadius;
        _attackDamageMin = _weaponItemDataSO.AttackDamageMin;
        _attackDamageMax = _weaponItemDataSO.AttackDamageMax;
        _knockbackForce = _weaponItemDataSO.KnockbackForce;
        _attackCooldownMax = _weaponItemDataSO.AttackCooldown;
        _attackCooldownCurrent = _attackCooldownMax;
    }

    private void OnDrawGizmos()
    {
        if (!_showGizmos)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_attackOrigin.transform.position, _attackRadius);
    }
}
