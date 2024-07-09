using UnityEngine;

[System.Serializable]
public class DashAbility : Ability
{
    private float _maxDashTime;
    private float _dashSpeed;

    private Vector2 _targetPosition;
    private float _currentDashTime;
    private AgentMovement _casterMovementModule;
    private AgentVisual _agentVisual;
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

        InitializeComponents();
    }

    public override void StartCast(Vector2 targetPosition)
    {
        if (OnCooldown || IsCasting)
            return;

        PutOnCooldown();
        IsCasting = true;

        _currentDashTime = _maxDashTime;
        _targetPosition = targetPosition;

        _agentVisual.EnableTrailRenderer();
        _casterMovementModule.DisableComponent();
        _casterAnimator.PlayAbilityAnimation(Name);
        
        _casterRigidBody.velocity = targetPosition.normalized * _dashSpeed;
    }

    public override void Tick()
    {
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

        _agentVisual.DisableTrailRenderer();
        _currentDashTime = _maxDashTime;
        IsCasting = false;
    }

    private void InitializeComponents()
    {
        _casterMovementModule = Caster.GetAgentComponent<AgentMovement>();
        _casterRigidBody = Caster.GetAgentRigidBody2D();
        _agentVisual = Caster.GetAgentComponent<AgentVisual>();
    }
}
