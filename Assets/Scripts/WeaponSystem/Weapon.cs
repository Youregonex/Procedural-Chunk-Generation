using UnityEngine;
using System;

public class Weapon : MonoBehaviour, IWeapon
{
    public event Action OnAttackStarted;
    public event Action OnAttackFinished;

    [Header("Config")]
    [SerializeField] private float _attackRadius;
    [SerializeField] private float _attackDamageMax;
    [SerializeField] private float _attackDamageMin;
    [SerializeField] private float _knockbackForce;
    [SerializeField] private WeaponAnimation _weaponAnimation;
    [SerializeField] private Transform _attackOrigin;

    [Header("Debug Fields")]
    [SerializeField] private AgentAttackModule _weaponHolder;
    [SerializeField] private bool _showGizmos;

    private void Awake()
    {
        _weaponHolder = transform.root.GetComponent<AgentAttackModule>();
    }

    private void Start()
    {
        _weaponAnimation.OnAttackAnimationStarted += WeaponAnimation_OnAttackAnimationStarted;
        _weaponAnimation.OnAttackAnimationFinished += WeaponAnimation_OnAttackAnimationFinished;
    }

    private void OnDestroy()
    {
        _weaponAnimation.OnAttackAnimationStarted -= WeaponAnimation_OnAttackAnimationStarted;
        _weaponAnimation.OnAttackAnimationFinished -= WeaponAnimation_OnAttackAnimationFinished;
    }

    public void Attack()
    {
        _weaponAnimation.PlayWeaponAttackAnimation();

        Collider2D[] targetsHit = Physics2D.OverlapCircleAll(_attackOrigin.position, _attackRadius);

        foreach(Collider2D hit in targetsHit)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();

            if (damageable == _weaponHolder.GetComponent<IDamageable>())
                continue;

            if (damageable != null)
            {
                damageable.TakeDamage(new DamageStruct
                {
                    damageSender = gameObject,
                    damageAmount = UnityEngine.Random.Range(_attackDamageMin, _attackDamageMax),
                    knockbackForce = _knockbackForce
                });
            }
        }
    }

    private void WeaponAnimation_OnAttackAnimationFinished()
    {
        OnAttackFinished?.Invoke();
    }

    private void WeaponAnimation_OnAttackAnimationStarted()
    {
        OnAttackStarted?.Invoke();
    }

    private void OnDrawGizmos()
    {
        if (!_showGizmos)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_attackOrigin.transform.position, _attackRadius);
    }
}
