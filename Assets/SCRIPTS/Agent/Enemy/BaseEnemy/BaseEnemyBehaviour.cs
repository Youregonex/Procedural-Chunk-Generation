using UnityEngine;
using System.Collections.Generic;
using System;
using Youregone.BehaviourTrees;
using Youregone.Utilities;

public class BaseEnemyBehaviour : AgentNetworkBehaviourComponent
{
    public event Action OnTargetInAttackRange;

    [field: Header("Config")]
    [field: SerializeField] public float AttackRangeMax { get; private set; }
    [field: SerializeField] public float AttackRangeMin { get; private set; }
    [field: SerializeField] public float AggroRange { get; private set; }
    [field: SerializeField] public float ChaseRange { get; private set; }
    [field: SerializeField] public float CombatRange { get; private set; }
    [field: SerializeField] public float AttackDelayMin { get; private set; }
    [field: SerializeField] public float AttackDelayMax { get; private set; }
    [field: SerializeField] public float TimeToStartRoamMin { get; private set; }
    [field: SerializeField] public float TimeToStartRoamMax { get; private set; }
    [field: SerializeField] public float RoamTimeMax { get; private set; }
    [field: SerializeField] public Vector2 RoamPositionOffsetMax { get; private set; }

    [field: Header("Debug Fields")]
    [field: SerializeField] public bool AgentSpawned { get; private set; }
    [field: SerializeField] public Vector2 CurrentRoamPosition { get; private set; }
    [field: SerializeField] public Vector2 MovementDirection { get; private set; }
    [field: SerializeField] public Vector2 AimPosition { get; private set; }
    [SerializeField] protected Transform _currentTargetTransform;
    [SerializeField] protected AgentTargetDetectionZone _targetDetectionZone;
    [SerializeField] protected AgentAttackModule _agentAttackModule;
    [SerializeField] protected bool _showGizmos;

    // BT
    protected Composite _behaviourTree;
    protected BTBuilder _btBuilder = new();

    public List<Transform> TargetTransformList => _targetDetectionZone.TargetList;
    public Transform CurrentTargetTransform => _currentTargetTransform;

    public override void Initialize()
    {
        GetAgentCore();

        _agentAttackModule = AgentCore.GetAgentComponent<AgentAttackModule>();
        AgentCore.GetAgentComponent<AgentAnimation>().OnAgentSpawned += AgentAnimation_OnAgentSpawned;
        InitializeAgentTargetDetectionZone();

        ConstructBehaviourTree();
    }

    protected virtual void Update()
    {
        PickCurrentTarget();

        _behaviourTree.Evaluate();
    }

    public override void OnNetworkDespawn()
    {
        AgentCore.GetAgentComponent<AgentAnimation>().OnAgentSpawned -= AgentAnimation_OnAgentSpawned;
    }

    public void SetAimPosition(Vector2 aimPosition) => AimPosition = aimPosition;
    public void SetMovementDirection(Vector2 movementDirection) => MovementDirection = movementDirection;
    public Vector2 SetRoamPosition(Vector2 newRoamPosition) => CurrentRoamPosition = newRoamPosition;
    public void SetCurrentTarget(Transform newTarget) => _currentTargetTransform = newTarget;

    public float GetAttackCooldown()
    {
        return _agentAttackModule.GetAttackCooldown();
    }

    public override void DisableComponent()
    {
        this.enabled = false;
    }

    public override void EnableComponent()
    {
        this.enabled = true;
    }

    public void TriggerAttack() => OnTargetInAttackRange?.Invoke();

    protected virtual void AgentAnimation_OnAgentSpawned()
    {
        AgentSpawned = true;
    }

    protected virtual void PickCurrentTarget()
    {
        if (_currentTargetTransform == null || _currentTargetTransform.GetComponent<AgentHealthSystem>().IsDead)
            _currentTargetTransform = TargetTransformList.Count == 0 ? null : TargetTransformList[0];

        if (_currentTargetTransform != null && !Utility.InRange(ChaseRange, transform.position, _currentTargetTransform.position))
            _currentTargetTransform = null;
    }

    protected virtual void ConstructBehaviourTree()
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

        Composite combatSelector =
            _btBuilder.StartBuildingSelector()
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

        _behaviourTree =
            _btBuilder.StartBuildingSelector()
            .WithSequence(enemySpawnSequence)
            .WithSequence(combatSequence)
            .WithSequence(roamSequence)
            .WithSequence(idleToRoamSelector)
            .Build();
    }

    protected void InitializeAgentTargetDetectionZone()
    {
        _targetDetectionZone = AgentCore.GetAgentComponent<AgentTargetDetectionZone>();
        _targetDetectionZone.SetDetectionZoneData(AggroRange);
    }

    protected virtual void OnDrawGizmos()
    {
        if (!_showGizmos)
            return;

        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(transform.position, AggroRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRangeMax);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, AttackRangeMin);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, CombatRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, ChaseRange);
    }
}
