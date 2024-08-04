using UnityEngine;
using Youregone.BehaviourTrees;

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
        Composite roamSequence =
            _btBuilder.StartBuildingSequence()
            .WithCondition(new RoamPositionExistsCondition(this))
            .WithInverter(new Inverter(new RoamTimerExpiredCondition(this, RoamTimeMax)))
            .WithBehaviour(new MoveToRoamPositionNode(this))
            .Build();

        Composite idleToRoamSelector =
            _btBuilder.StartBuildingSelector()
            .WithBehaviour(new IdleToRoamNode(this, TimeToStartRoamMin, TimeToStartRoamMax, RoamPositionOffsetMax))
            .Build();

        Composite attackSequnce =
            _btBuilder.StartBuildingSequence()
            .WithCondition(new TargetInRangeCondition(this, AttackRangeMax))
            .WithCondition(new AttackOffCooldownCondition(this))
            .WithBehaviour(new AttackNode(this, AttackDelayMin, AttackDelayMax))
            .Build();

        Composite chaseSequence =
            _btBuilder.StartBuildingSequence()
            .WithCondition(new TargetInRangeCondition(this, ChaseRange))
            .WithBehaviour(new ChaseNode(this, AttackRangeMin, AttackRangeMax))
            .Build();

        Composite dashSequence =
            _btBuilder.StartBuildingSequence()
            .WithInverter(new Inverter(new TargetInRangeCondition(this, AttackRangeMax)))
            .WithCondition(new AbilityOffCooldownCondition(_agentAbilitySystem, "DASH"))
            .WithBehaviour(new CastDashToTargetNode(this, "DASH", _agentAbilitySystem))
            .Build();

        Composite combatSelector =
            _btBuilder.StartBuildingSelector()
            .WithSequence(dashSequence)
            .WithSequence(attackSequnce)
            .WithSequence(chaseSequence)
            .Build();

        Composite combatSequence =
            _btBuilder.StartBuildingSequence()
            .WithCondition(new TargetExistsCondition(this))
            .WithSelector(combatSelector)
            .Build();

        Composite enemySpawnSequence =
            _btBuilder.StartBuildingSequence()
            .WithInverter(new Inverter(new AgentSpawnedCondition(this)))
            .WithSelector(idleToRoamSelector)
            .Build();

        Composite treeRoot =
            _btBuilder.StartBuildingSelector()
            .WithSequence(enemySpawnSequence)
            .WithSequence(combatSequence)
            .WithSequence(roamSequence)
            .WithSequence(idleToRoamSelector)
            .Build();

        _behaviourTree = (Selector)treeRoot;
    }
}