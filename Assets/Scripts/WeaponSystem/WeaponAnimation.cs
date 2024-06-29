using UnityEngine;
using System;

public class WeaponAnimation : MonoBehaviour
{
    private const string ATTACK = "ATTACK";

    public event Action OnAttackAnimationStarted;
    public event Action OnAttackAnimationFinished;

    [Header("Debug Field")]
    [SerializeField] private Animator _weaponAnimator;

    private void Awake()
    {
        _weaponAnimator = GetComponent<Animator>();
    }

    public void PlayWeaponAttackAnimation()
    {
        _weaponAnimator.SetTrigger(ATTACK);
    }

    private void AttackAnimationStarted() // Used by Animation Event
    {
        OnAttackAnimationStarted?.Invoke();
    }

    private void AttackAnimationFinished() // Used by Animation Event
    {
        OnAttackAnimationFinished?.Invoke();
    }
}
