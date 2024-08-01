using UnityEngine;
using System;

[Serializable]
public abstract class Ability
{
    public event Action OnCastCompleted;

    public AgentCoreBase Caster { get; protected set; }
    public string Name { get; protected set; }
    public EAbilityType AbilityType { get; protected set; }
    public float Cooldown { get; protected set; }
    public GameObject AbilityParticles { get; private set; }

    public float CurrentCooldown { get; protected set; }
    public bool IsCasting { get; protected set; }

    protected AgentAnimation _casterAnimator;
    protected Action<Transform> _callbackAction;

    public bool OnCooldown => CurrentCooldown > 0;


    public Ability(AgentCoreBase caster,
                   AgentAnimation casterAnimator,
                   string name,
                   EAbilityType abilityType,
                   float cooldown,
                   GameObject abilityParticles,
                   Action<Transform> callbackAction)
    {
        Caster = caster;
        _casterAnimator = casterAnimator;
        Name = name;
        AbilityType = abilityType;
        Cooldown = cooldown;
        AbilityParticles = abilityParticles;
        _callbackAction = callbackAction;

        CurrentCooldown = 0;
        IsCasting = false;
    }


    public virtual void StartCast(Vector2 targetPosition) // Call base method from overrided methods for proper work
    {
        if (OnCooldown || IsCasting)
            return;

        PutOnCooldown();
        IsCasting = true;
    }

    public virtual void StopCast() // Call base method from overrided methods for proper work
    {
        IsCasting = false;
        CastCompleted();
    }

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
