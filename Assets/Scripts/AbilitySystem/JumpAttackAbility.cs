using UnityEngine;

public class JumpAttackAbility : Ability
{
    private const string JUMP_ATTACK_START = "JUMP_ATTACK_START";
    private const string JUMP_ATTACK_LAND = "JUMP_ATTACK_LAND";

    private float _maxTimeInAir;
    private float _impactDamage;
    private float _impactRange;
    private float _airborneSpeed;

    private AgentMovement _casterMovementModule;
    private Rigidbody2D _casterRigidBody;
    private AgentHitbox _agentHitbox;
    private BaseEnemyBehaviour _enemyBehaviour;
    private AgentVisual _agentVisual;
    private AgentStats _agentStats;

    private bool _inAir;
    private bool _landing;
    private float _currentTimeInAir;
    private float _proximityThreshold;
    private float _baseSpeed;


    public JumpAttackAbility(AgentCoreBase caster,
                             AgentAnimation casterAnimator,
                             string name,
                             EAbilityType abilityType,
                             float cooldown,
                             GameObject abilityParticles,
                             float maxTimeInAir,
                             float impactDamage,
                             float impactRange,
                             float proximityThreshold,
                             float airborneSpeed) : base(caster, casterAnimator, name, abilityType, cooldown, abilityParticles)
    {
        _maxTimeInAir = maxTimeInAir;
        _impactDamage = impactDamage;
        _impactRange = impactRange;
        _proximityThreshold = proximityThreshold;
        _currentTimeInAir = _maxTimeInAir;
        _airborneSpeed = airborneSpeed;

        InitializeComponents();
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
                Vector2.Distance(Caster.transform.position, _enemyBehaviour.GetCurrentTargetTransform().position) <= _proximityThreshold)
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

            _agentVisual.DisableShadow();
            GameObject.Instantiate(AbilityParticles, Caster.transform.position, Quaternion.identity);

            Collider2D[] colliders = Physics2D.OverlapCircleAll(Caster.transform.position, _impactRange);

            foreach(Collider2D collider in colliders)
            {
                if(collider.TryGetComponent(out IDamageable damageable))
                {
                    if (ReferenceEquals(_agentHitbox, damageable))
                        continue;

                    DamageStruct damageStruct = new DamageStruct
                    {
                        damageSender = Caster.gameObject,
                        damageAmount = _impactDamage,
                        knockbackForce = 0f
                    };

                    damageable.TakeDamage(damageStruct);
                }
            }

            StopCast();
            _casterAnimator.OnAnimationEnded -= CasterAnimator_OnAnimationEnded;
            return;
        }

        _inAir = true;
        _agentVisual.EnableShadow();
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

        _baseSpeed = _agentStats.GetCurrentStatValue(EStats.MoveSpeed);
        _agentStats.ModifyStatCurrentValue(EStats.MoveSpeed, _airborneSpeed);
    }

    private void StartLanding()
    {
        _landing = true;
        _inAir = false;
        _casterRigidBody.velocity = Vector2.zero;

        _casterMovementModule.DisableComponent();
        _agentHitbox.EnableComponent();

        _casterAnimator.PlayAbilityAnimation(JUMP_ATTACK_LAND);

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
