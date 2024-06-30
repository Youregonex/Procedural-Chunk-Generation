using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
public class CharacterHealthSystem : HealthSystem
{
    private CharacterStats _characterStats;

    [Header("Config")]
    [SerializeField] private float _vitalityToHealthRatio;
    [SerializeField] private float _initialHealth;

    private void Start()
    {
        _characterStats = GetComponent<CharacterStats>();

        float vitalityValue = _characterStats.GetCurrentStatValue(EStats.Vitality);
        _maxHealth = _initialHealth + (vitalityValue * _vitalityToHealthRatio);
        _currentHealth = _maxHealth;
    }


}
