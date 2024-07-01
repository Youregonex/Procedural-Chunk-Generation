using UnityEngine;

[RequireComponent(typeof(AgentInput))]
public class AgentAttackModule : AgentMonobehaviourComponent
{
    [Header("Config")]
    [SerializeField] protected float _attackCooldownMax = 1f;

    [Header("Debug Fields")]
    [SerializeField] protected float _attackCooldownCurrent;
    [SerializeField] protected bool _canAttack = true;
    [SerializeField] protected Weapon _currentWeapon;
    [SerializeField] protected AgentInput _agentInput;
    [SerializeField] protected AgentCoreBase _agentCore;

    private void Awake()
    {
        _agentCore = GetComponent<AgentCoreBase>();
    }

    private void Start()
    {
        _agentInput = _agentCore.GetAgentComponent<AgentInput>();

        _agentInput.OnAgentAttackTriggered += AgentInput_OnAgentAttackTrigger;
    }

    private void Update()
    {
        if (_attackCooldownCurrent > 0)
            _attackCooldownCurrent -= Time.deltaTime;
    }

    private void OnDestroy()
    {
        if(_agentInput != null)
            _agentInput.OnAgentAttackTriggered -= AgentInput_OnAgentAttackTrigger;

        if (_currentWeapon != null)
        {
            _currentWeapon.OnAttackStarted -= Weapon_OnAttackStarted;
            _currentWeapon.OnAttackFinished -= Weapon_OnAttackFinished;
        }
    }

    public override void DisableComponent()
    {
        this.enabled = false;
    }

    private void AgentInput_OnAgentAttackTrigger(object sender, System.EventArgs e)
    {
        Attack();
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
