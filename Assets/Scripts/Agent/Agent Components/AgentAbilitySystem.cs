using UnityEngine;
using System.Collections.Generic;

public class AgentAbilitySystem : AgentMonobehaviourComponent
{
    [Header("Config")]
    [SerializeField] protected List<Ability> _abilityList = new List<Ability>();

    [Header("Debug Fields")]
    [SerializeField] protected AgentCoreBase _agentCore;
    [SerializeField] protected Ability _currentAbility;

    public bool CastingAbility => _currentAbility != null;

    protected virtual void Awake()
    {
        _agentCore = GetComponent<AgentCoreBase>();
    }

    protected virtual void Update()
    {
        if (_currentAbility != null)
            _currentAbility.Tick();

        foreach(Ability ability in _abilityList)
        {
            ability.CooldownTick();
        }
    }

    public void CastAbility(Ability ability, Vector2 targetPosition)
    {
        if (ability.CurrentCooldown > 0)
            return;

        Debug.Log($"Casting {ability.Name}");

        if (_currentAbility != null)
        {
            Debug.LogError("Current cast not null!");
            return;
        }


        _currentAbility = ability;
        Debug.Log($"Current Ability : {_currentAbility.Name}");
        _currentAbility.OnCastCompleted += Ability_OnCastCompleted;
        _currentAbility.StartCast(targetPosition);
    }

    protected void Ability_OnCastCompleted()
    {
        Debug.Log($"{_currentAbility.Name} completed");
        _currentAbility.OnCastCompleted -= Ability_OnCastCompleted;
        _currentAbility = null;
    }

    public override void DisableComponent()
    {
        this.enabled = false;
    }

    public override void EnableComponent()
    {
        this.enabled = true;
    }
}
