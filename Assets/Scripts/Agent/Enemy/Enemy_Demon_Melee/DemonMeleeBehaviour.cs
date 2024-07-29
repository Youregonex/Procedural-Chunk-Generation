using System.Collections.Generic;
using UnityEngine;

public class DemonMeleeBehaviour : BaseEnemyBehaviour
{
    [Header("Debug Fields")]
    [SerializeField] private AgentAbilitySystem _agentAbilitySystem;


    protected override void Start()
    {
        _agentAbilitySystem = _enemyCore.GetAgentComponent<AgentAbilitySystem>();

        base.Start();
    }

    protected override void ConstructBehaviourTree()
    {
        RoamPositionExistsCondition roamPositionExistsCondition = new RoamPositionExistsCondition(this);
        RoamTimerExpiredCondition roamTimerExpiredCondition = new RoamTimerExpiredCondition(this, RoamTimeMax);
        Inverter roamTimerExpiredConditionInverter = new Inverter(roamTimerExpiredCondition);
        MoveToRoamPositionNode moveToRoamPositionNode = new MoveToRoamPositionNode(this);
        Sequence roamSequence = new Sequence(new List<Node> { roamPositionExistsCondition, roamTimerExpiredConditionInverter, moveToRoamPositionNode });

        IdleNode idleNode = new IdleNode(this, TimeToStartRoamMin, TimeToStartRoamMax, RoamPositionOffsetMax);

        TargetInRangeCondition targetInAttackRangeCondition = new TargetInRangeCondition(this, AttackRangeMax);
        AttackOffCooldownCondition attackOffCooldownCondition = new AttackOffCooldownCondition(this);
        AttackNode attackNode = new AttackNode(this, AttackDelayMin, AttackDelayMax);
        Sequence attackSequnce = new Sequence(new List<Node> { targetInAttackRangeCondition, attackOffCooldownCondition, attackNode });

        TargetInChaseRangeCondition targetInChaseRangeCondition = new TargetInChaseRangeCondition(this, ChaseRange);
        ChaseNode chaseNode = new ChaseNode(this, AttackRangeMin, AttackRangeMax);
        Sequence chaseSequence = new Sequence(new List<Node> { targetInChaseRangeCondition, chaseNode });

        Inverter targetInAttackRangeInvertor = new Inverter(targetInAttackRangeCondition);
        AbilityOffCooldownCondition abilityOffCooldownCondition = new AbilityOffCooldownCondition(_agentAbilitySystem, "DASH");
        CastDashToTargetNode castDashToTargetNode = new CastDashToTargetNode(this, "DASH", _agentAbilitySystem);
        Sequence dashSequence = new Sequence(new List<Node>() { targetInAttackRangeInvertor, abilityOffCooldownCondition, castDashToTargetNode });

        TargetExistsCondition targetExistsCondition = new TargetExistsCondition(this);
        Selector combatSelector = new Selector(new List<Node> { dashSequence, attackSequnce, chaseSequence });
        Sequence combatSequence = new Sequence(new List<Node> { targetExistsCondition, combatSelector });

        AgentSpawnedCondition agentSpawnedCondition = new AgentSpawnedCondition(this);
        Inverter agentSpawnedInverter = new Inverter(agentSpawnedCondition);
        Sequence enemySpawnSequence = new Sequence(new List<Node> { agentSpawnedInverter, idleNode });

        Selector treeRoot = new Selector(new List<Node> { enemySpawnSequence, combatSequence, roamSequence, idleNode });

        _behaviourTree = treeRoot;
    }
}
