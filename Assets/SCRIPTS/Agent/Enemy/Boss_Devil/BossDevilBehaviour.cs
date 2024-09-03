using UnityEngine;
using Youregone.BehaviourTrees;

public class BossDevilBehaviour : BaseEnemyBehaviour
{
    [Header("Config")]
    [SerializeField] private ShootPatternDataSO _radial360AttackDataSO;

    [Header("Debug Fields")]
    [SerializeField] private float _jumpAttackRangeGizmos;
    [SerializeField] private AgentAbilitySystem _agentAbilitySystem;

    private BulletShooter _bulletShooter;

    public BulletShooter BulletShooter => _bulletShooter;

    public override void Initialize()
    {
        GetAgentCore();

        _bulletShooter = new BulletShooter(_radial360AttackDataSO, transform, this);
        _agentAbilitySystem = AgentCore.GetAgentComponent<AgentAbilitySystem>();

        _agentAttackModule = AgentCore.GetAgentComponent<AgentAttackModule>();
        AgentCore.GetAgentComponent<AgentAnimation>().OnAgentSpawned += AgentAnimation_OnAgentSpawned;
        InitializeAgentTargetDetectionZone();

        ConstructBehaviourTree();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    protected override void ConstructBehaviourTree()
    {
        Composite spawnSequence =
            _btBuilder.StartBuildingSequence()
            .WithInverter(new Inverter(new AgentSpawnedCondition(this)))
            .WithBehaviour(new IdleToRoamNode(this, TimeToStartRoamMin, TimeToStartRoamMax, RoamPositionOffsetMax))
            .Build();

        Composite idleToRoamSelector =
            _btBuilder.StartBuildingSelector()
            .WithBehaviour(new IdleToRoamNode(this, TimeToStartRoamMin, TimeToStartRoamMax, RoamPositionOffsetMax))
            .Build();

        Composite attackSequence =
            _btBuilder.StartBuildingSequence()
            .WithCondition(new TargetInRangeCondition(this, AttackRangeMax))
            .WithCondition(new AnyAbilityOffCooldownCondition(_agentAbilitySystem))
            .WithBehaviour(new CastFirstOffCooldownAbilityNode(_agentAbilitySystem))
            .Build();

        Composite chaseSequence =
            _btBuilder.StartBuildingSequence()
            .WithCondition(new TargetInRangeCondition(this, ChaseRange))
            .WithBehaviour(new ChaseNode(this, AttackRangeMin, AttackRangeMax))
            .Build();

        Composite combatSelector =
            _btBuilder.StartBuildingSelector()
            .WithCondition(new IsCastingCondition(_agentAbilitySystem))
            .WithSequence(attackSequence)
            .WithSequence(chaseSequence)
            .Build();

        Composite combatSequence =
            _btBuilder.StartBuildingSequence()
            .WithCondition(new TargetExistsCondition(this))
            .WithSelector(combatSelector)
            .Build();

        Composite roamSequence =
            _btBuilder.StartBuildingSequence()
            .WithCondition(new RoamPositionExistsCondition(this))
            .WithInverter(new Inverter(new RoamTimerExpiredCondition(this, RoamTimeMax)))
            .WithBehaviour(new MoveToRoamPositionNode(this))
            .Build();

        Composite idleSequence =
            _btBuilder.StartBuildingSequence()
            .WithInverter(new Inverter(new IsCastingCondition(_agentAbilitySystem)))
            .WithSelector(idleToRoamSelector)
            .Build();

        _behaviourTree =
            _btBuilder.StartBuildingSelector()
            .WithSequence(spawnSequence)
            .WithSequence(combatSequence)
            .WithSequence(roamSequence)
            .WithSequence(idleSequence)
            .Build();
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (!_showGizmos)
            return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, _jumpAttackRangeGizmos);
    }
}