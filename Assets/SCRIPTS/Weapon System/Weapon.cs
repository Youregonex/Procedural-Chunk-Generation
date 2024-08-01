using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] protected WeaponItemDataSO _weaponItemDataSO;
    [SerializeField] protected WeaponAnimation _weaponAnimation;
    [SerializeField] protected Transform _attackOrigin;

    [Header("Debug Fields")]
    [SerializeField] protected float _attackDamageMax;
    [SerializeField] protected float _attackDamageMin;
    [SerializeField] protected float _knockbackForce;
    [SerializeField] protected float _attackCooldownMax;
    [SerializeField] protected float _attackCooldownCurrent = 0f;
    [SerializeField] protected EnemyCore _agentCore;
    [SerializeField] protected AgentAttackModule _agentAttackModule;
    [SerializeField] protected ItemHoldPoint _itemHoldPoint;
    [SerializeField] protected bool _showGizmos;

    public bool ReadyToAttack => _attackCooldownCurrent <= 0;
    public float AttackCooldownCurrent => _attackCooldownCurrent;


    protected void Update()
    {
        if(_attackCooldownCurrent > 0)
            _attackCooldownCurrent -= Time.deltaTime;
    }

    public void SetupWeapon(EnemyCore agentCore, AgentAttackModule agentAttackModule, ItemHoldPoint itemHoldPoint)
    {
        _agentCore = agentCore;
        _agentAttackModule = agentAttackModule;
        _itemHoldPoint = itemHoldPoint;

        SetupWeaponStats();
    }

    public virtual void Attack() { }

    protected virtual void SetupWeaponStats()
    {
        _attackDamageMin = _weaponItemDataSO.AttackDamageMin;
        _attackDamageMax = _weaponItemDataSO.AttackDamageMax;
        _knockbackForce = _weaponItemDataSO.KnockbackForce;
        _attackCooldownMax = _weaponItemDataSO.AttackCooldown;
        _attackCooldownCurrent = _attackCooldownMax;
    }
}
