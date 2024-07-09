using UnityEngine;

[System.Serializable]
public class DashAbility : Ability
{
    private float _maxDashTime;
    private float _dashSpeed;

    private Vector2 _targetPosition;
    private float _currentDashTime;
    private AgentMovement _casterMovementModule;
    private Rigidbody2D _casterRigidBody;

    public DashAbility(AgentCoreBase caster,
                       AgentAnimation casterAnimator,
                       string name,
                       EAbilityType abilityType,
                       float cooldown,
                       float maxDashTime,
                       float dashSpeed) : base(caster, casterAnimator, name, abilityType, cooldown)
    {
        _maxDashTime = maxDashTime;
        _dashSpeed = dashSpeed;

        _currentDashTime = _maxDashTime;
        Debug.Log($"MaxDashTime: {_maxDashTime}");
    }

    public override void StartCast(Vector2 targetPosition)
    {
        if (OnCooldown || IsCasting)
            return;

        Debug.Log($"{Name} StartCast");
        PutOnCooldown();
        IsCasting = true;

        _currentDashTime = _maxDashTime;
        _targetPosition = targetPosition;
        _casterMovementModule = Caster.GetAgentComponent<AgentMovement>();
        _casterRigidBody = Caster.GetComponent<Rigidbody2D>();

        _casterMovementModule.DisableComponent();
        _casterAnimator.PlayAbilityAnimation(Name);

        Vector2 movementDirection = (targetPosition - (Vector2)Caster.transform.position).normalized;
        _casterRigidBody.velocity = movementDirection * _dashSpeed;
    }

    public override void Tick()
    {
        Debug.Log($"{Name} DashTime: {_currentDashTime} | Tick");
        if (_currentDashTime <= 0)
        {
            StopCast();
            return;
        }

        _currentDashTime -= Time.deltaTime;
    }

    public override void StopCast()
    {
        AgentMovement agentMovement = Caster.GetAgentComponent<AgentMovement>();
        agentMovement.EnableComponent();

        Caster.GetComponent<Rigidbody2D>().velocity = Vector2.zero; 
        CastCompleted();

        _currentDashTime = _maxDashTime;
        IsCasting = false;
    }
}
