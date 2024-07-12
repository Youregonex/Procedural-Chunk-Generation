using UnityEngine;
using System;

[Serializable]
public class Ability
{
    public event Action OnCastCompleted;

    public AgentCoreBase Caster { get; protected set; }
    protected AgentAnimation _casterAnimator;
    public string Name { get; protected set; }
    public EAbilityType AbilityType { get; protected set; }
    public float Cooldown { get; protected set; } = 0;

    public float CurrentCooldown { get; protected set; }
    public bool IsCasting { get; protected set; }


    public bool OnCooldown => CurrentCooldown > 0;

    public Ability(AgentCoreBase caster, AgentAnimation casterAnimator, string name, EAbilityType abilityType, float cooldown)
    {
        Caster = caster;
        _casterAnimator = casterAnimator;
        Name = name;
        AbilityType = abilityType;
        Cooldown = cooldown;

        CurrentCooldown = 0;
        IsCasting = false;
    }

    public virtual void StartCast(Vector2 targetPosition) { }
    public virtual void StopCast() { }

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
