using System.Collections.Generic;
using UnityEngine;

public class BossDevilBehaviour : BaseEnemyBehaviour
{
    [Header("Debug Fields")]
    [SerializeField] private float _jumpAttackRange;
    [SerializeField] private AgentAbilitySystem _agentAbilitySystem;


    protected override void Start()
    {
        _agentAbilitySystem = _enemyCore.GetAgentComponent<AgentAbilitySystem>();

        base.Start();
    }

    protected override void ConstructBehaviourTree()
    {
        AgentSpawnedCondition agentSpawnedCondition = new AgentSpawnedCondition(this);
        Inverter agentSpawnedConditionInverter = new Inverter(agentSpawnedCondition);
        IdleNode idleNode = new IdleNode(this, TimeToStartRoamMin, TimeToStartRoamMax, RoamPositionOffsetMax);
        Sequence spawnSequence = new Sequence(new List<Node> { agentSpawnedConditionInverter, idleNode });

        TargetInAttackRangeCondition targetInAttackRangeCondition = new TargetInAttackRangeCondition(this, AttackRangeMax);
        AnyAbilityOffCooldownCondition anyAbilityOffCooldownCondition = new AnyAbilityOffCooldownCondition(_agentAbilitySystem);
        CastAbilityNode castAbilityNode = new CastAbilityNode(_agentAbilitySystem);
        Sequence attackSequence = new Sequence(new List<Node> { targetInAttackRangeCondition, anyAbilityOffCooldownCondition, castAbilityNode });

        TargetInChaseRangeCondition targetInChaseRangeCondition = new TargetInChaseRangeCondition(this, ChaseRange);
        ChaseNode chaseNode = new ChaseNode(this, AttackRangeMin, AttackRangeMax);
        Sequence chaseSequence = new Sequence(new List<Node> { targetInChaseRangeCondition, chaseNode });

        TargetExistsCondition targetExistsCondition = new TargetExistsCondition(this);
        Selector combatSelector = new Selector(new List<Node> { attackSequence, chaseSequence });
        Sequence combatSequence = new Sequence(new List<Node> { targetExistsCondition, combatSelector });

        RoamPositionExistsCondition roamPositionExistsCondition = new RoamPositionExistsCondition(this);
        RoamTimerExpiredCondition roamTimerExpiredCondition = new RoamTimerExpiredCondition(this, RoamTimeMax);
        Inverter roamTimerExpiredConditionInverter = new Inverter(roamTimerExpiredCondition);
        MoveToRoamPositionNode moveToRoamPositionNode = new MoveToRoamPositionNode(this);
        Sequence roamSequence = new Sequence(new List<Node> { roamPositionExistsCondition, roamTimerExpiredConditionInverter, moveToRoamPositionNode });

        IsCastingCondition isCastingCondition = new IsCastingCondition(_agentAbilitySystem);
        Inverter isCastingConditionInverter = new Inverter(isCastingCondition);
        Sequence idleSequence = new Sequence(new List<Node> { isCastingConditionInverter, idleNode });

        _behaviourTree = new Selector(new List<Node> {spawnSequence, combatSequence, roamSequence, idleSequence });
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (!_showGizmos)
            return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, _jumpAttackRange);
    }
}
