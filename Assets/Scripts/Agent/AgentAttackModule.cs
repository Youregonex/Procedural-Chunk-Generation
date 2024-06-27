using UnityEngine;

[RequireComponent(typeof(AgentInput))]
public class AgentAttackModule : MonoBehaviour
{
    [SerializeField] protected float _attackCooldownCurrent;
    [SerializeField] protected float _attackCooldownMax = 1f;
    [SerializeField] private float _attackRadius;
    [SerializeField] private AgentAnimation _agentAnimation;

    private AgentInput _agentInput;
    protected bool _canAttack = true;

    [Header("Debug Fields")]
    [SerializeField] protected Weapon _currentWeapon;

    private void Awake()
    {
        _agentInput = GetComponent<AgentInput>();
    }

    private void Start()
    {
        _agentInput.OnAgentAttackTriggered += AgentInput_OnAgentAttackTrigger;
    }

    private void Update()
    {
        if (_attackCooldownCurrent > 0)
            _attackCooldownCurrent -= Time.deltaTime;
    }

    private void OnDestroy()
    {
        _agentInput.OnAgentAttackTriggered -= AgentInput_OnAgentAttackTrigger;

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

    private void Weapon_OnAttackFinished(object sender, System.EventArgs e)
    {
        _canAttack = true;
    }

    private void Weapon_OnAttackStarted(object sender, System.EventArgs e)
    {
        _canAttack = false;
    }
}
