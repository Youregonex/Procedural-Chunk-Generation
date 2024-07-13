using System.Collections.Generic;
using UnityEngine;

public class DemonRangedBehaviour : BaseEnemyBehaviour
{
    [Header("Debug Fields")]
    [SerializeField] private AgentAbilitySystem _demonAbilitySystem;


    protected override void Start()
    {
        _demonAbilitySystem = _enemyCore.GetAgentComponent<AgentAbilitySystem>();

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

        TargetInAttackRangeCondition targetInAttackRangeCondition = new TargetInAttackRangeCondition(this, AttackRangeMax);
        AttackOffCooldownCondition attackOffCooldownCondition = new AttackOffCooldownCondition(this);
        AttackNode attackNode = new AttackNode(this, AttackDelayMin, AttackDelayMax);
        Sequence attackSequnce = new Sequence(new List<Node> { targetInAttackRangeCondition, attackOffCooldownCondition, attackNode });

        TargetInChaseRangeCondition targetInChaseRangeCondition = new TargetInChaseRangeCondition(this, ChaseRange);
        ChaseNode chaseNode = new ChaseNode(this, AttackRangeMin, AttackRangeMax);
        Sequence chaseSequence = new Sequence(new List<Node> { targetInChaseRangeCondition, chaseNode });

        TargetTooCloseCondition targetTooCloseCondition = new TargetTooCloseCondition(this, AttackRangeMin);
        AbilityOffCooldownCondition abilityOffCooldownCondition = new AbilityOffCooldownCondition(_demonAbilitySystem, "DASH");
        CastDashFromTargetNode castDashFromTargetNode = new CastDashFromTargetNode(this, "DASH", _demonAbilitySystem);
        Sequence dashSequence = new Sequence(new List<Node>() { targetTooCloseCondition, abilityOffCooldownCondition, castDashFromTargetNode });

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
