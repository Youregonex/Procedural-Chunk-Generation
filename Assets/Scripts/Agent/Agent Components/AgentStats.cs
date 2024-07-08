using System.Collections.Generic;
using UnityEngine;
using System;

public class AgentStats : AgentMonobehaviourComponent
{
    [SerializeField] private StatConfigSO _initialStatConfigSO;

    public event EventHandler<OnStatChangedEventArgs> OnStatChanged;
    public class OnStatChangedEventArgs : EventArgs
    {
        public EStats changedStat;
        public float oldValue;
        public float newValue;
    }

    private Dictionary<EStats, Stat> _characterStatDictionary;

    private void Awake()
    {
        _characterStatDictionary = new Dictionary<EStats, Stat>();

        InitializeStatDictionary();
    }

    public override void DisableComponent()
    {
        throw new NotImplementedException();
    }

    public void LogStats()
    {
        foreach (Stat stat in _characterStatDictionary.Values)
        {
            Debug.Log($"Stat {stat.statName}\n" +
          $"Max Value : {stat.maxValue}\n" +
          $"Current Value : {stat.currentValue}\n" +
          $"Min Value : {stat.minValue}\n");
        }
    }

    public Dictionary<EStats, Stat> GetCharacterStatDictionary() => _characterStatDictionary;

    public bool TryModifyStatCurrentValue(EStats statToModify, float newStatValue)
    {
        if (!_characterStatDictionary.ContainsKey(statToModify))
        {
            Debug.LogError($"{gameObject} does not have {statToModify} stat!");

            return false;
        }

        Stat stat = _characterStatDictionary[statToModify];

        float oldStatValue = stat.currentValue;

        _characterStatDictionary[statToModify].currentValue = Mathf.Clamp(
                                                                        newStatValue,
                                                                        stat.minValue,
                                                                        stat.maxValue);

        OnStatChanged?.Invoke(this, new OnStatChangedEventArgs
        {
            changedStat = statToModify,
            oldValue = oldStatValue,
            newValue = newStatValue
        });

        return true;
    }

    public float GetCurrentStatValue(EStats stat)
    {
        if (!_characterStatDictionary.ContainsKey(stat))
            return -1f;

        return _characterStatDictionary[stat].currentValue;
    }

    private void InitializeStatDictionary()
    {
        foreach(Stat stat in _initialStatConfigSO.stats)
        {
            if (_characterStatDictionary.ContainsKey(stat.statName))
                continue;

            _characterStatDictionary.Add(stat.statName, new Stat
            {
                statName = stat.statName,
                maxValue = stat.maxValue,
                minValue = stat.minValue,
                currentValue = stat.currentValue
            });
        }
    }
}
