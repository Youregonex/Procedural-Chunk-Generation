using UnityEngine;

public abstract class AbilityDataSO : ScriptableObject
{
    [field: SerializeField] public string Name { get; protected set; }
    [field: SerializeField] public EAbilityType AbilityType { get; protected set; }
    [field: SerializeField] public float Cooldown { get; private set; }

    public abstract Ability BuildAbility(AgentCoreBase caster, AgentAnimation casterAnimation);
}
