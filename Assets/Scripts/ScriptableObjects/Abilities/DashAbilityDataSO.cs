using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Dash Ability Data")]
public class DashAbilityDataSO : AbilityDataSO
{
    [field: SerializeField] public float MaxDashTime { get; private set; }
    [field: SerializeField] public float DashSpeed { get; private set; }

    public override Ability BuildAbility(AgentCoreBase caster, AgentAnimation casterAnimation)
    {
        DashAbility dashAbility = new DashAbility(caster, casterAnimation, Name, AbilityType, Cooldown, MaxDashTime, DashSpeed);

        return dashAbility;
    }
}
