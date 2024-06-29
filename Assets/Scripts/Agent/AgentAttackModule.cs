using UnityEngine;

[RequireComponent(typeof(AgentInput))]
public class AgentAttackModule : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private AgentAnimation _agentAnimation;

    private AgentInput _agentInput;
    protected bool _canAttack = true;

    [Header("Debug Fields")]
    [SerializeField] protected Weapon _currentWeapon;
    [SerializeField] protected float _attackCooldownMax = 1f;
    [SerializeField] protected float _attackCooldownCurrent;
    [SerializeField] private HealthSystem _healthSystem;

    private void Awake()
    {
        _agentInput = GetComponent<AgentInput>();
        _healthSystem = GetComponent<HealthSystem>();
    }

    private void Start()
    {
        _agentInput.OnAgentAttackTriggered += AgentInput_OnAgentAttackTrigger;
        _healthSystem.OnDeath += HealthSystem_OnDeath;
    }

    private void Update()
    {
        if (_attackCooldownCurrent > 0)
            _attackCooldownCurrent -= Time.deltaTime;
    }

    private void OnDestroy()
    {
        _agentInput.OnAgentAttackTriggered -= AgentInput_OnAgentAttackTrigger;
        _healthSystem.OnDeath -= HealthSystem_OnDeath;

        if (_currentWeapon != null)
        {
            _currentWeapon.OnAttackStarted -= Weapon_OnAttackStarted;
            _currentWeapon.OnAttackFinished -= Weapon_OnAttackFinished;
        }
    }

    private void AgentInput_OnAgentAttackTrigger(object sender, System.EventArgs e)
    {
        Attack();
    }

    private void HealthSystem_OnDeath()
    {
        this.enabled = false;
    }

    protected virtual void Attack()
    {
        if (_attackCooldownCurrent > 0 || !_canAttack)
            return;

        _attackCooldownCurrent = _attackCooldownMax;

        _currentWeapon.Attack();
    }

    private void GetNewWeapon(Weapon newWeapon)
    {
        if(_currentWeapon != null)
        {
            _currentWeapon.OnAttackStarted -= Weapon_OnAttackStarted;
            _currentWeapon.OnAttackFinished -= Weapon_OnAttackFinished;
        }

        _currentWeapon = newWeapon;

        _currentWeapon.OnAttackStarted += Weapon_OnAttackStarted;
        _currentWeapon.OnAttackFinished += Weapon_OnAttackFinished;
    }

    private void Weapon_OnAttackFinished()
    {
        _canAttack = true;
    }

    private void Weapon_OnAttackStarted()
    {
        _canAttack = false;
    }
}
