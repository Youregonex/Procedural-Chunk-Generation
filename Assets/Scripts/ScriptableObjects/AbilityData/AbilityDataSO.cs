using UnityEngine;
using System;

public abstract class AbilityDataSO : ScriptableObject
{
    [field: SerializeField] public string AbilityName { get; protected set; }
    [field: SerializeField] public EAbilityType AbilityType { get; protected set; }
    [field: SerializeField] public float Cooldown { get; private set; }
    [field: SerializeField] public GameObject AbilityParticles { get; private set; }
    [field: SerializeField] public bool HasCallback { get; private set; }

    public abstract Ability BuildAbility(AgentCoreBase caster, AgentAnimation casterAnimation, Action<Transform> callbackAction = null);
}
