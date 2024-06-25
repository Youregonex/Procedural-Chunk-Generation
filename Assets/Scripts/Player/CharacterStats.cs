using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] private StatConfigSO _initialStatConfigSO;

    public event EventHandler<OnStatChangedEventArgs> OnStatChanged;
    public class OnStatChangedEventArgs : EventArgs
    {
        public StatsEnum changedStat;
        public float oldValue;
        public float newValue;
    }

    private Dictionary<StatsEnum, Stat> _characterStatDictionary;

    private void Awake()
    {
        _characterStatDictionary = new Dictionary<StatsEnum, Stat>();

        InitializeStatDictionary();
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

    public bool TryModifyStatCurrentValue(StatsEnum statToModify, float newStatValue)
    {
        if (!_characterStatDictionary.ContainsKey(statToModify))
        {
            Debug.LogError($"{gameObject} does not have {statToModify} stat!");

            return false;
        }

        Stat stat = _characterStatDictionary[statToModify];

        float oldStatValue = stat.currentValue;

        _characterStatDictionary[statToModify].currentValue = Mathf.Clamp(newStatValue,
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

    public float GetCurrentStatValue(StatsEnum stat)
    {
        if (!_characterStatDictionary.ContainsKey(stat))
            return -1f;

        return _characterStatDictionary[stat].currentValue;
    }

    public Dictionary<StatsEnum, Stat> GetCharacterStatDictionary() => _characterStatDictionary;

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
