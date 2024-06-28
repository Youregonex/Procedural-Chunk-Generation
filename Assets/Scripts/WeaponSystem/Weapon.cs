using UnityEngine;
using System;

public class Weapon : MonoBehaviour, IWeapon
{
    private const string ATTACK = "ATTACK";

    public event EventHandler OnAttackStarted;
    public event EventHandler OnAttackFinished;

    [SerializeField] private Animator _weaponAnimator;
    [SerializeField] private Transform _attackOrigin;

    [Header("Debug Fields")]
    [SerializeField] private float _attackRadius;
    [SerializeField] private float _attackDamageMax;
    [SerializeField] private float _attackDamageMin;
    [SerializeField] private float _knockbackForce;
    [SerializeField] private AgentAttackModule _weaponHolder;
    [SerializeField] private bool _showGizmos;


    public void Attack()
    {
        _weaponAnimator.SetTrigger(ATTACK);

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

    private void AttackStarted() // Used by Animation Event
    {
        OnAttackStarted?.Invoke(this, EventArgs.Empty);
    }

    private void AttackFinished() // Used by Animation Event
    {
        OnAttackFinished?.Invoke(this, EventArgs.Empty);
    }

    private void OnDrawGizmos()
    {
        if (!_showGizmos)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_attackOrigin.transform.position, _attackRadius);
    }
}
