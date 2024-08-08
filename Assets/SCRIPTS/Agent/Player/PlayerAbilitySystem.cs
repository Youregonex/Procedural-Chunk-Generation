using UnityEngine;
using System;

public class PlayerAbilitySystem : AgentAbilitySystem
{
    public event Action<AbilityDataSO, Ability> OnAbilityAdded;
    public event Action<AbilityDataSO, Ability> OnAbilityRemoved;

    [Header("Config")]
    [SerializeField] private AgentMovement _agentMovement;

    private bool _isInitialized = false;


    public void Initialize()
    {
        if (_isInitialized)
            return;

        _isInitialized = true;

        BuildAbilityCallbacks();
        BuildAbilities();
    }

    protected override void Start() {} // Stops from building abilities on Start

    protected override void Update()
    {
        base.Update();

        if(Input.GetKeyDown(KeyCode.Space))
        {
            CastAbility(_abilityDictionary["DASH"], _agentMovement.LastMovementDirection);
        }
    }

    protected override void BuildAbilities()
    {
        foreach (AbilityDataSO abilityDataSO in _abilityDataSOList)
        {
            Ability ability = null;

            if (abilityDataSO.HasCallback)
            {
                Action<Transform> abilityCallback = _abilityCallbacksDictionary[abilityDataSO.AbilityName.ToUpper()];
                ability = abilityDataSO.BuildAbility(_agentCore, _agentCore.GetAgentComponent<AgentAnimation>(), abilityCallback);

                OnAbilityAdded?.Invoke(abilityDataSO, ability);
            }
            else
            {
                ability = abilityDataSO.BuildAbility(_agentCore, _agentCore.GetAgentComponent<AgentAnimation>());

                OnAbilityAdded?.Invoke(abilityDataSO, ability);
            }

            _abilityDictionary.Add(ability.AbilityName.ToUpper(), ability);
        }
    }

    private void RemoveAbility()
    {
        OnAbilityRemoved?.Invoke(null, null); // TODO: Remove ability system
    }
}