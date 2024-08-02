using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Abilities/Dash Ability Data")]
public class DashAbilityDataSO : AbilityDataSO
{
    [field: SerializeField] public float MaxDashTime { get; private set; }
    [field: SerializeField] public float DashSpeed { get; private set; }

    public override Ability BuildAbility(AgentCoreBase caster, AgentAnimation casterAnimation, Action<Transform> callbackAction)
    {
        DashAbility dashAbility = new DashAbility(caster,
                                                  casterAnimation,
                                                  this,
                                                  AbilityName,
                                                  AbilityType,
                                                  Cooldown,
                                                  AbilityParticles,
                                                  MaxDashTime,
                                                  DashSpeed,
                                                  callbackAction);

        return dashAbility;
    }
}
