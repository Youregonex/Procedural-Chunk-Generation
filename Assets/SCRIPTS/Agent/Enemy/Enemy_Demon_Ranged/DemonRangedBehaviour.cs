using UnityEngine;
using Youregone.BehaviourTrees;

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

        Composite attackSequence =
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
            .WithCondition(new TargetInRangeCondition(this, AttackRangeMin))
            .WithCondition(new AbilityOffCooldownCondition(_demonAbilitySystem, "DASH"))
            .WithBehaviour(new CastDashFromTargetNode(this, "DASH", _demonAbilitySystem))
            .Build();

        Composite combatSelector =
            _btBuilder.StartBuildingSelector()
            .WithSequence(dashSequence)
            .WithSequence(attackSequence)
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
