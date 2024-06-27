using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
public class CharacterHealthSystem : HealthSystem
{
    private CharacterStats _characterStats;

    [Header("Debug Fields")]
    [SerializeField] private float _vitalityToHealthRatio;
    [SerializeField] private float _initialHealth;

    protected override void Awake()
    {

    }

    private void Start()
    {
        _characterStats = GetComponent<CharacterStats>();

        float vitalityValue = _characterStats.GetCurrentStatValue(StatsEnum.Vitality);
        _maxHealth = _initialHealth + (vitalityValue * _vitalityToHealthRatio);
        _currentHealth = _maxHealth;
    }


}
