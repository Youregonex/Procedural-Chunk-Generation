using UnityEngine;

public class AbilityDataSO : ScriptableObject
{
    [field: SerializeField] public string Name { get; protected set; }
    [field: SerializeField] public EAbilityType AbilityType { get; protected set; }
    [field: SerializeField] public float Cooldown { get; private set; }

    public virtual Ability BuildAbility(AgentCoreBase caster, AgentAnimation casterAnimation)
    {
        Ability ability = new Ability(caster, casterAnimation, Name, AbilityType, Cooldown);

        return ability;
    }
}
