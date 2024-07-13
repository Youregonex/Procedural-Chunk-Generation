using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/JumpAttack Ability Data")]
public class JumpAttackDataSO : AbilityDataSO
{
    [field: SerializeField] public float MaxTimeInAir { get; private set; }
    [field: SerializeField] public float ImpactDamage { get; private set; }
    [field: SerializeField] public float ImpactRange { get; private set; }

    public override Ability BuildAbility(AgentCoreBase caster, AgentAnimation casterAnimation)
    {
        JumpAttackAbility jumpAttackAbility = new JumpAttackAbility(caster,
                                                                    casterAnimation,
                                                                    Name,
                                                                    AbilityType,
                                                                    Cooldown,
                                                                    MaxTimeInAir,
                                                                    ImpactDamage,
                                                                    ImpactRange);
        return jumpAttackAbility;
    }
}
