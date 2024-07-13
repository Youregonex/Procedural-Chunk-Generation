using UnityEngine;

public class JumpAttackAbility : Ability
{
    private const string JUMP_ATTACK_START = "JUMP_ATTACK_START";
    private const string JUMP_ATTACK_LAND = "JUMP_ATTACK_LAND";

    private float _maxTimeInAir;
    private float _impactDamage;
    private float _impactRange;

    private float _currentTimeInAir;
    private AgentMovement _casterMovementModule;
    private Rigidbody2D _casterRigidBody;
    private AgentHitbox _agentHitbox;
    private BaseEnemyBehaviour _enemyBehaviour;

    private bool _inAir;
    private bool _landing;


    public JumpAttackAbility(AgentCoreBase caster,
                             AgentAnimation casterAnimator,
                             string name,
                             EAbilityType abilityType,
                             float cooldown,
                             float maxTimeInAir,
                             float impactDamage,
                             float impactRange) : base(caster, casterAnimator, name, abilityType, cooldown)
    {
        _maxTimeInAir = maxTimeInAir;
        _impactDamage = impactDamage;
        _impactRange = impactRange;

        _currentTimeInAir = _maxTimeInAir;

        InitializeComponents();
    }

    public override void StartCast(Vector2 targetPosition)
    {
        if (OnCooldown || IsCasting)
            return;

        PutOnCooldown();
        IsCasting = true;

        Debug.Log("Cast Started");
        StartJumpingAttack();
    }

    public override void Tick()
    {
        Debug.Log($"Current Air Time: {_currentTimeInAir}");
        if(_inAir)
        {
            Vector2 movementDirection = (_enemyBehaviour.GetCurrentTargetTransform().position - Caster.transform.position).normalized;
            Debug.Log($"Movement Direction: {movementDirection}");
            _enemyBehaviour.SetMovementDirection(movementDirection);

            if (_currentTimeInAir <= 0)
            {
                StartLanding();
            }
            else
                _currentTimeInAir -= Time.deltaTime;
        }
    }

    public override void StopCast()
    {
        _casterMovementModule.EnableComponent();

        CastCompleted();

        ResetAbilityState();

        IsCasting = false;
    }

    private void CasterAnimator_OnAnimationEnded()
    {
        if(_landing)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(Caster.transform.position, _impactRange);

            foreach(Collider2D collider in colliders)
            {
                IDamageable damageable = collider.GetComponent<IDamageable>();

                if(damageable != null)
                {
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

        if (!_inAir)
        {
            Debug.Log("In Air");
            _inAir = true;
            _casterMovementModule.EnableComponent();
            return;
        }
    }

    private void StartJumpingAttack()
    {
        Debug.Log("Jump Started");
        _casterMovementModule.DisableComponent();
        _casterRigidBody.velocity = Vector2.zero;
        _agentHitbox.DisableComponent();

        _casterAnimator.OnAnimationEnded += CasterAnimator_OnAnimationEnded;
        _casterAnimator.PlayAbilityAnimation(JUMP_ATTACK_START);
    }

    private void StartLanding()
    {
        Debug.Log("Landed");

        _landing = true;
        _inAir = false;
        _casterMovementModule.DisableComponent();
        _casterRigidBody.velocity = Vector2.zero;
        _agentHitbox.EnableComponent();

        _casterAnimator.PlayAbilityAnimation(JUMP_ATTACK_LAND);
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
        _casterRigidBody = Caster.GetAgentRigidBody2D();
        _agentHitbox = Caster.GetAgentComponent<AgentHitbox>();
    }
}
