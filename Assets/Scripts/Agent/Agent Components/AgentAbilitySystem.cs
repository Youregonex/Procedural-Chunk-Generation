using UnityEngine;
using System.Collections.Generic;

public class AgentAbilitySystem : AgentMonobehaviourComponent
{
    [Header("Config")]
    [SerializeField] protected List<AbilityDataSO> _abilityDataSOList;

    [Header("Debug Fields")]
    [SerializeField] protected AgentCoreBase _agentCore;
    [SerializeField] protected Ability _currentAbility;
    [SerializeField] protected Dictionary<string, Ability> _abilityDictionary = new Dictionary<string, Ability>();

    public bool CastingAbility => _currentAbility != null;

    protected virtual void Awake()
    {
        _agentCore = GetComponent<AgentCoreBase>();
    }

    protected void Start()
    {
        BuildAbilities();
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
        _currentAbility = ability;
        _currentAbility.OnCastCompleted += Ability_OnCastCompleted;
        _currentAbility.StartCast(targetPosition);
    }

    public void CastAbility(string abilityName, Vector2 targetPosition)
    {
        _currentAbility = _abilityDictionary[abilityName];
        _currentAbility.OnCastCompleted += Ability_OnCastCompleted;
        _currentAbility.StartCast(targetPosition);
    }

    protected void BuildAbilities()
    {
        foreach(AbilityDataSO abilityDataSO in _abilityDataSOList)
        {
            Ability ability = abilityDataSO.BuildAbility(_agentCore, _agentCore.GetAgentComponent<AgentAnimation>());
            _abilityDictionary.Add(ability.Name.ToUpper(), ability);
        }
    }

    protected void Ability_OnCastCompleted()
    {
        _currentAbility.OnCastCompleted -= Ability_OnCastCompleted;
        _currentAbility = null;
    }
}
