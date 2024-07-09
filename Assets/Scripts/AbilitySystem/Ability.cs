using UnityEngine;
using System;

[System.Serializable]
public abstract class Ability
{
    public event Action OnCastCompleted;

    public AgentCoreBase Caster { get; protected set; }
    public string Name { get; protected set; }
    public EAbilityType AbilityType { get; protected set; }
    public float Cooldown { get; protected set; } = 0;
    public float CurrentCooldown { get; protected set; }
    public bool IsCasting { get; protected set; }

    protected AgentAnimation _casterAnimator;

    public bool OnCooldown => CurrentCooldown > 0;

    public Ability(AgentCoreBase caster, AgentAnimation casterAnimator, string name, EAbilityType abilityType, float cooldown)
    {
        Caster = caster;
        _casterAnimator = casterAnimator;
        Name = name;
        AbilityType = abilityType;
        Cooldown = cooldown;
    }

    public abstract void StartCast(Vector2 targetPosition);
    public abstract void StopCast();

    public virtual void Tick() {}

    public void CooldownTick()
    {
        if (CurrentCooldown > 0 && !IsCasting)
        {
            CurrentCooldown -= Time.deltaTime;
        }
    }

    protected void PutOnCooldown() => CurrentCooldown = Cooldown;
    protected void CastCompleted() => OnCastCompleted?.Invoke();
}
