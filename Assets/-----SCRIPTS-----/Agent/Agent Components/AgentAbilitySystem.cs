using UnityEngine;
using System.Collections.Generic;
using System;

public class AgentAbilitySystem : AgentMonobehaviourComponent
{
    [Header("Config")]
    [SerializeField] protected List<AbilityDataSO> _abilityDataSOList;

    [Header("Debug Fields")]
    [SerializeField] protected AgentCoreBase _agentCore;
    [SerializeField] protected Ability _currentAbility;
    [SerializeField] protected Dictionary<string, Ability> _abilityDictionary = new Dictionary<string, Ability>();

    protected Dictionary<string, Action<Transform>> _abilityCallbacksDictionary = new Dictionary<string, Action<Transform>>();

    public Dictionary<string, Ability> AbilityDictionary => _abilityDictionary; // All ability names in upper-case
    public bool IsCastingAbility => _currentAbility != null;


    protected virtual void Awake()
    {
        _agentCore = GetComponent<AgentCoreBase>();
        _currentAbility = null;
    }

    protected void Start()
    {
        BuildAbilityCallbacks();
        BuildAbilities();
    }

    protected virtual void Update()
    {
        AbilityTick();
        AbilityCooldownTick();
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
        StartCast(ability.Name.ToUpper(), targetPosition);
    }

    public void CastAbility(string abilityName, Vector2 targetPosition)
    {
        StartCast(abilityName, targetPosition);
    }

    private void StartCast(string abilityName, Vector2 targetPosition)
    {
        if (!_abilityDictionary.ContainsKey(abilityName.ToUpper()))
        {
            Debug.LogError($"{gameObject.name} does not have {abilityName} ability!");
            return;
        }

        if (_abilityDictionary[abilityName.ToUpper()].OnCooldown || IsCastingAbility)
            return;

        _currentAbility = _abilityDictionary[abilityName.ToUpper()];
        _currentAbility.OnCastCompleted += Ability_OnCastCompleted;
        _currentAbility.StartCast(targetPosition);
    }

    protected void AbilityTick()
    {
        if (_currentAbility != null)
            _currentAbility.Tick();
    }

    protected void AbilityCooldownTick()
    {
        foreach (KeyValuePair<string, Ability> keyValuePair in _abilityDictionary)
        {
            if (keyValuePair.Value.OnCooldown)
                keyValuePair.Value.CooldownTick();
        }
    }

    protected virtual void BuildAbilityCallbacks() { } // Override in children to add Ability callbacks

    protected void AddAbilityCallback(string abilityName, Action<Transform> callback) // Used to add callbacks to Abilities
    {
        _abilityCallbacksDictionary.Add(abilityName, callback);
    }

    protected void BuildAbilities()
    {
        foreach(AbilityDataSO abilityDataSO in _abilityDataSOList)
        {
            Ability ability = null;

            if(abilityDataSO.HasCallback)
            {
                Action<Transform> abilityCallback = _abilityCallbacksDictionary[abilityDataSO.AbilityName.ToUpper()];

                ability = abilityDataSO.BuildAbility(_agentCore, _agentCore.GetAgentComponent<AgentAnimation>(), abilityCallback);
            }
            else
                ability = abilityDataSO.BuildAbility(_agentCore, _agentCore.GetAgentComponent<AgentAnimation>());
            

            _abilityDictionary.Add(ability.Name.ToUpper(), ability);
        }
    }

    protected void Ability_OnCastCompleted()
    {
        _currentAbility.OnCastCompleted -= Ability_OnCastCompleted;
        _currentAbility = null;
    }
}