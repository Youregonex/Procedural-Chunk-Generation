using UnityEngine;
using System;

public class DashAbility : Ability
{
    private float _maxDashTime;
    private float _dashSpeed;

    private float _currentDashTime;
    private AgentMovement _casterMovementModule;
    private AgentVisual _agentVisual;
    private Rigidbody2D _casterRigidBody;

    public DashAbility(AgentCoreBase caster,
                       AgentAnimation casterAnimator,
                       string name,
                       EAbilityType abilityType,
                       float cooldown,
                       GameObject abilityParticles,
                       float maxDashTime,
                       float dashSpeed,
                       Action<Transform> callbackAction) : base(caster, casterAnimator, name, abilityType, cooldown, abilityParticles, callbackAction)
    {
        _maxDashTime = maxDashTime;
        _dashSpeed = dashSpeed;

        _currentDashTime = _maxDashTime;

        InitializeComponents();
    }

    public override void StartCast(Vector2 targetPosition)
    {
        base.StartCast(targetPosition);

        _currentDashTime = _maxDashTime;

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
        base.StopCast();

        AgentMovement agentMovement = Caster.GetAgentComponent<AgentMovement>();
        agentMovement.EnableComponent();

        Caster.GetComponent<Rigidbody2D>().velocity = Vector2.zero; 

        _agentVisual.DisableTrailRenderer();
        _currentDashTime = _maxDashTime;
    }

    protected void InitializeComponents()
    {
        _casterMovementModule = Caster.GetAgentComponent<AgentMovement>();
        _casterRigidBody = Caster.GetAgentRigidBody();
        _agentVisual = Caster.GetAgentComponent<AgentVisual>();
    }
}
