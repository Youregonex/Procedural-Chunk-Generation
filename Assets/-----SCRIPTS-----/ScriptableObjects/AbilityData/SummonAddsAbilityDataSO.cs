using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Abilities/Summon Adds Ability Data")]
public class SummonAddsAbilityDataSO : AbilityDataSO
{
    [field: SerializeField] public int MaxAddsCount { get; private set; }
    [field: SerializeField] public int AddsSummonPerCast { get; private set; }
    [field: SerializeField] public List<AgentCoreBase> AddsPrefabList { get; private set; }

    public override Ability BuildAbility(AgentCoreBase caster, AgentAnimation casterAnimation, Action<Transform> callbackAction)
    {
        SummonAddsAbility summonAddsAbility = new SummonAddsAbility(caster,
                                                                    casterAnimation,
                                                                    AbilityName,
                                                                    AbilityType,
                                                                    Cooldown,
                                                                    AbilityParticles,
                                                                    AddsSummonPerCast,
                                                                    MaxAddsCount,
                                                                    AddsPrefabList,
                                                                    callbackAction);
        return summonAddsAbility;
    }
}
