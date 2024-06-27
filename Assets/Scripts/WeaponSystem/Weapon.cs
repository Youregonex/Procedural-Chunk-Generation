using UnityEngine;
using System;

public class Weapon : MonoBehaviour, IWeapon
{
    private const string ATTACK = "ATTACK";

    public event EventHandler OnAttackStarted;
    public event EventHandler OnAttackFinished;

    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _attackOrigin;

    [Header("Debug Fields")]
    [SerializeField] private float _attackRadius;
    [SerializeField] private float _attackDamage;

    public void Attack()
    {
        _animator.SetTrigger(ATTACK);

        Collider2D[] targetsHit = Physics2D.OverlapCircleAll(_attackOrigin.position, _attackRadius);

        foreach(Collider2D hit in targetsHit)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();

            if (damageable != null)
            {
                Debug.Log($"{damageable} hit");
                damageable.TakeDamage(new DamageStruct
                {
                    damageSender = gameObject,
                    damageAmount = _attackDamage
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_attackOrigin.transform.position, _attackRadius);
    }
}
