using UnityEngine;
using DG.Tweening;
using System;
using Youregone.Utilities;

public class JumpAttackAbility : Ability
{
    private const string JUMP_ATTACK_START = "JUMP_ATTACK_START";
    private const string JUMP_ATTACK_LAND = "JUMP_ATTACK_LAND";

    private float _maxTimeInAir;
    private float _airborneSpeed;

    private AgentMovement _casterMovementModule;
    private Rigidbody2D _casterRigidBody;
    private AgentHitbox _agentHitbox;
    private BaseEnemyBehaviour _enemyBehaviour;
    private AgentVisual _agentVisual;
    private AgentStats _agentStats;

    private Transform _shadowTransform;
    private bool _inAir;
    private bool _landing;
    private float _currentTimeInAir;
    private float _proximityThreshold;
    private float _baseSpeed;

    private float _shadowGrowthTime = 1f;
    private Vector2 _shadowZeroScale = Vector2.zero;
    private Vector2 _shadowInitialScale;

    public JumpAttackAbility(AgentCoreBase caster,
                             AgentAnimation casterAnimator,
                             string name,
                             EAbilityType abilityType,
                             float cooldown,
                             GameObject abilityParticles,
                             float maxTimeInAir,
                             float proximityThreshold,
                             float airborneSpeed,
                             Action<Transform> callbackAction) : base(caster, casterAnimator, name, abilityType, cooldown, abilityParticles, callbackAction)
    {
        _maxTimeInAir = maxTimeInAir;
        _proximityThreshold = proximityThreshold;
        _currentTimeInAir = _maxTimeInAir;
        _airborneSpeed = airborneSpeed;

        InitializeComponents();
        _shadowTransform = _agentVisual.GetShadowGameObject().transform;
        _shadowInitialScale = _shadowTransform.transform.localScale;
    }

    public override void StartCast(Vector2 targetPosition)
    {
        base.StartCast(targetPosition);
        StartJumpingAttack();
    }

    public override void Tick()
    {
        if(_inAir)
        {
            Vector2 movementDirection = (_enemyBehaviour.GetCurrentTargetTransform().position - Caster.transform.position).normalized;
            _enemyBehaviour.SetMovementDirection(movementDirection);

            if (_currentTimeInAir <= 0 ||
                Utility.InRange(_proximityThreshold, Caster.transform.position, _enemyBehaviour.GetCurrentTargetTransform().position))
            {
                StartLanding();
            }
            else
                _currentTimeInAir -= Time.deltaTime;
        }
    }

    public override void StopCast()
    {
        base.StopCast();

        _casterMovementModule.EnableComponent();
        ResetAbilityState();
    }

    private void CasterAnimator_OnAnimationEnded()
    {
        if(_landing)
        {
            Caster.EnableCollider();
            _agentHitbox.EnableComponent();

            _callbackAction?.Invoke(_enemyBehaviour.GetCurrentTargetTransform());

            StopCast();
            _casterAnimator.OnAnimationEnded -= CasterAnimator_OnAnimationEnded;
            return;
        }

        _inAir = true;
        _casterMovementModule.EnableComponent();

    }

    private void StartJumpingAttack()
    {
        _casterMovementModule.DisableComponent();
        _agentHitbox.DisableComponent();
        Caster.DisableCollider();

        _casterRigidBody.velocity = Vector2.zero;
        _inAir = true;

        _casterAnimator.OnAnimationEnded += CasterAnimator_OnAnimationEnded;
        _casterAnimator.PlayAbilityAnimation(JUMP_ATTACK_START);

        _shadowTransform.localScale = Vector2.zero;
        _agentVisual.EnableShadow();
        float _shadowGrowthDelay = .7f;
        _shadowTransform.DOScale(_shadowInitialScale, _shadowGrowthTime).SetDelay(_shadowGrowthDelay);

        _baseSpeed = _agentStats.GetCurrentStatValue(EStats.MoveSpeed);
        _agentStats.ModifyStatCurrentValue(EStats.MoveSpeed, _airborneSpeed);
    }

    private void StartLanding()
    {
        _landing = true;
        _inAir = false;
        _casterRigidBody.velocity = Vector2.zero;

        _casterMovementModule.DisableComponent();
        _casterAnimator.PlayAbilityAnimation(JUMP_ATTACK_LAND);

        _shadowTransform.DOScale(_shadowZeroScale, _shadowGrowthTime).OnComplete(() =>
        {
            _agentVisual.DisableShadow();
        });

        _agentStats.ModifyStatCurrentValue(EStats.MoveSpeed, _baseSpeed);
    }

    private void ResetAbilityState()
    {
        _currentTimeInAir = _maxTimeInAir;
        _inAir = false;
        _landing = false;
    }

    protected void InitializeComponents()
    {
        _enemyBehaviour = Caster.GetAgentComponent<BaseEnemyBehaviour>();
        _casterMovementModule = Caster.GetAgentComponent<AgentMovement>();
        _casterRigidBody = Caster.GetAgentRigidBody();
        _agentHitbox = Caster.GetAgentComponent<AgentHitbox>();
        _agentVisual = Caster.GetAgentComponent<AgentVisual>();
        _agentStats = Caster.GetAgentComponent<AgentStats>();
    }
}
