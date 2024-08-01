using System.Collections.Generic;
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

    protected override void Awake()
    {
        base.Awake();

        _bulletShooter = new BulletShooter(_radial360AttackDataSO, transform, this);
    }

    protected override void Start()
    {
        _agentAbilitySystem = _enemyCore.GetAgentComponent<AgentAbilitySystem>();

        base.Start();
    }

    protected override void ConstructBehaviourTree()
    {
        AgentSpawnedCondition agentSpawnedCondition = new AgentSpawnedCondition(this);
        Inverter agentSpawnedConditionInverter = new Inverter(agentSpawnedCondition);
        IdleToRoamNode idleToRoamNode = new IdleToRoamNode(this, TimeToStartRoamMin, TimeToStartRoamMax, RoamPositionOffsetMax);
        Sequence spawnSequence = new Sequence(new List<Node> { agentSpawnedConditionInverter, idleToRoamNode });

        TargetInRangeCondition targetInAttackRangeCondition = new TargetInRangeCondition(this, AttackRangeMax);
        AnyAbilityOffCooldownCondition anyAbilityOffCooldownCondition = new AnyAbilityOffCooldownCondition(_agentAbilitySystem);
        CastFirstOffCooldownAbilityNode castFirstOffCooldownAbilityNode = new CastFirstOffCooldownAbilityNode(_agentAbilitySystem);
        Sequence attackSequence = new Sequence(new List<Node> { targetInAttackRangeCondition, anyAbilityOffCooldownCondition, castFirstOffCooldownAbilityNode });

        TargetInChaseRangeCondition targetInChaseRangeCondition = new TargetInChaseRangeCondition(this, ChaseRange);
        ChaseNode chaseNode = new ChaseNode(this, AttackRangeMin, AttackRangeMax);
        Sequence chaseSequence = new Sequence(new List<Node> { targetInChaseRangeCondition, chaseNode });

        IsCastingCondition isCastingCondition = new IsCastingCondition(_agentAbilitySystem);
        TargetExistsCondition targetExistsCondition = new TargetExistsCondition(this);
        Selector combatSelector = new Selector(new List<Node> { isCastingCondition, attackSequence, chaseSequence });
        Sequence combatSequence = new Sequence(new List<Node> { targetExistsCondition, combatSelector });

        RoamPositionExistsCondition roamPositionExistsCondition = new RoamPositionExistsCondition(this);
        RoamTimerExpiredCondition roamTimerExpiredCondition = new RoamTimerExpiredCondition(this, RoamTimeMax);
        Inverter roamTimerExpiredConditionInverter = new Inverter(roamTimerExpiredCondition);
        MoveToRoamPositionNode moveToRoamPositionNode = new MoveToRoamPositionNode(this);
        Sequence roamSequence = new Sequence(new List<Node> { roamPositionExistsCondition, roamTimerExpiredConditionInverter, moveToRoamPositionNode });

        Inverter isCastingConditionInverter = new Inverter(isCastingCondition);
        Sequence idleSequence = new Sequence(new List<Node> { isCastingConditionInverter, idleToRoamNode });

        _behaviourTree = new Selector(new List<Node> {spawnSequence, combatSequence, roamSequence, idleSequence });
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
