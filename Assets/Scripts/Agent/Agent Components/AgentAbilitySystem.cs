using UnityEngine;
using System.Collections.Generic;

public class AgentAbilitySystem : AgentMonobehaviourComponent
{
    [Header("Config")]
    [SerializeField] protected Dictionary<string, Ability> _abilityDictionary = new Dictionary<string, Ability>();

    [Header("Debug Fields")]
    [SerializeField] protected EnemyCore _agentCore;
    [SerializeField] protected Ability _currentAbility;

    public bool CastingAbility => _currentAbility != null;

    protected virtual void Awake()
    {
        _agentCore = GetComponent<EnemyCore>();
    }

    protected virtual void Update()
    {
        if (_currentAbility != null)
            _currentAbility.Tick();

        foreach(KeyValuePair<string, Ability> keyValuePair in _abilityDictionary)
        {
            if (keyValuePair.Value.OnCooldown)
                keyValuePair.Value.CooldownTick();
        }
    }

    public bool IsOnCooldown(string abilityName)
    {
        if (!_abilityDictionary.ContainsKey(abilityName))
            return true;

        return _abilityDictionary[abilityName].OnCooldown;
    }

    public override void DisableComponent()
    {
        this.enabled = false;
    }

    public override void EnableComponent()
    {
        this.enabled = true;
    }

    public void CastAbility(Ability ability, Vector2 targetPosition)
    {
        if (_currentAbility != null || IsOnCooldown(ability.Name))
            return;

        _currentAbility = ability;
        _currentAbility.OnCastCompleted += Ability_OnCastCompleted;
        _currentAbility.StartCast(targetPosition);
    }

    public void CastAbility(string abilityName, Vector2 targetPosition)
    {
        if (_currentAbility != null || IsOnCooldown(abilityName))
            return;

        _currentAbility = _abilityDictionary[abilityName];
        _currentAbility.OnCastCompleted += Ability_OnCastCompleted;
        _currentAbility.StartCast(targetPosition);
    }

    protected void Ability_OnCastCompleted()
    {
        _currentAbility.OnCastCompleted -= Ability_OnCastCompleted;
        _currentAbility = null;
    }
}
