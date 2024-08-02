using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Abilities/JumpAttack Ability Data")]
public class JumpAttackAbilityDataSO : AbilityDataSO
{
    [field: SerializeField] public float MaxTimeInAir { get; private set; }
    [field: SerializeField] public float ProximityThreshold { get; private set; }
    [field: SerializeField] public float AirborneSpeed { get; private set; }

    public override Ability BuildAbility(AgentCoreBase caster, AgentAnimation casterAnimation, Action<Transform> callbackAction = null)
    {
        JumpAttackAbility jumpAttackAbility = new JumpAttackAbility(caster,
                                                                    casterAnimation,
                                                                    AbilityName,
                                                                    AbilityType,
                                                                    Cooldown,
                                                                    AbilityParticles,
                                                                    MaxTimeInAir,
                                                                    ProximityThreshold,
                                                                    AirborneSpeed,
                                                                    callbackAction);
        return jumpAttackAbility;
    }
}
