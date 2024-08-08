using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AbilityCooldownUIDisplay : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private List<AbilityCooldownUI> _abilityCooldownUIList;

    [Header("Debug Fields")]
    [SerializeField] private PlayerAbilitySystem _playerAbilitySystem;
    [SerializeField] private List<Ability> _currentAbilityList = new List<Ability>();
    [SerializeField] private Dictionary<AbilityDataSO, AbilityCooldownUI> _abilitiesToDisplayDictionary = new Dictionary<AbilityDataSO, AbilityCooldownUI>();


    public void Initialize(PlayerAbilitySystem playerAbilitySystem)
    {
        _playerAbilitySystem = playerAbilitySystem;

        foreach (KeyValuePair<AbilityDataSO, AbilityCooldownUI> keyValuePair in _abilitiesToDisplayDictionary)
        {
            keyValuePair.Value.HideElement();
        }

        _playerAbilitySystem.OnAbilityAdded += PlayerAbilitySystem_OnAbilityAdded;
        _playerAbilitySystem.OnAbilityRemoved += PlayerAbilitySystem_OnAbilityRemoved;
    }

    private void OnDestroy()
    {
        if(_currentAbilityList.Count != 0)
            foreach (Ability ability in _currentAbilityList)
            {
                ability.OnCooldownTick -= Ability_OnCooldownTick;
            }

        _playerAbilitySystem.OnAbilityAdded -= PlayerAbilitySystem_OnAbilityAdded;
        _playerAbilitySystem.OnAbilityRemoved -= PlayerAbilitySystem_OnAbilityRemoved;
    }

    private void PlayerAbilitySystem_OnAbilityRemoved(AbilityDataSO abilityDataSO, Ability ability)
    {
        _abilitiesToDisplayDictionary[abilityDataSO].HideElement();
        _abilitiesToDisplayDictionary.Remove(abilityDataSO);

        Ability abilityToRemove = _currentAbilityList.FirstOrDefault(t => t == ability);

        if (abilityToRemove == null)
        {
            Debug.LogError($"Can't remove {ability.AbilityName}");
            return;
        }

        abilityToRemove.OnCooldownTick -= Ability_OnCooldownTick;
        _currentAbilityList.Remove(ability);
    }

    private void PlayerAbilitySystem_OnAbilityAdded(AbilityDataSO abilityDataSO, Ability ability)
    {
        AbilityCooldownUI abilityCooldownUI = _abilityCooldownUIList.FirstOrDefault(t => !t.isActiveAndEnabled);

        if (abilityCooldownUI == null)
        {
            Debug.LogError($"Spell display is full! Can't add {abilityDataSO.AbilityName} ability cooldown UI!");
            return;
        }

        abilityCooldownUI.SetupElement(abilityDataSO.AbilityIcon);
        abilityCooldownUI.UpdateFillAmount(1f - (ability.CurrentCooldown / ability.Cooldown));
        abilityCooldownUI.ShowElement();

        _abilitiesToDisplayDictionary.Add(abilityDataSO, abilityCooldownUI);

        _currentAbilityList.Add(ability);
        ability.OnCooldownTick += Ability_OnCooldownTick;
    }

    private void Ability_OnCooldownTick(Ability abilityTicked, float currentCooldown, float maxCooldown)
    {
        _abilitiesToDisplayDictionary[abilityTicked.AbilityDataSO].UpdateFillAmount(1f - (currentCooldown / maxCooldown));
    }
}