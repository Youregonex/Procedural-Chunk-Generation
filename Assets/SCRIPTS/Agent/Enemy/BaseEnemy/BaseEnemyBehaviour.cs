using UnityEngine;
using System.Collections.Generic;
using System;
using Youregone.BehaviourTrees;
using Youregone.Utilities;

public class BaseEnemyBehaviour : AgentMonobehaviourComponent
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
    [field: SerializeField] public bool IsSpawned { get; private set; }
    [field: SerializeField] public Vector2 CurrentRoamPosition { get; private set; }
    [field: SerializeField] public Vector2 MovementDirection { get; private set; }
    [field: SerializeField] public Vector2 AimPosition { get; private set; }
    [SerializeField] protected Transform _currentTargetTransform;
    [SerializeField] protected AgentTargetDetectionZone _targetDetectionZone;
    [SerializeField] protected AgentAttackModule _agentAttackModule;
    [SerializeField] protected EnemyCore _enemyCore;
    [SerializeField] protected bool _showGizmos;

    protected Selector _behaviourTree;

    public List<Transform> TargetTransformList => _targetDetectionZone.TargetList;

    protected virtual void Awake()
    {
        _enemyCore = GetComponent<EnemyCore>();
    }

    protected virtual void Start()
    {
        _agentAttackModule = _enemyCore.GetAgentComponent<AgentAttackModule>();
        _enemyCore.GetAgentComponent<AgentAnimation>().OnAgentSpawned += AgentAnimation_OnAgentSpawned;
        InitializeAgentTargetDetectionZone();

        ConstructBehaviourTree();
    }

    protected virtual void Update()
    {
        PickCurrentTarget();

        _behaviourTree.Evaluate();
    }

    protected void OnDestroy()
    {
        _enemyCore.GetAgentComponent<AgentAnimation>().OnAgentSpawned -= AgentAnimation_OnAgentSpawned;
    }

    public void SetAimPosition(Vector2 aimPosition) => AimPosition = aimPosition;
    public void SetMovementDirection(Vector2 movementDirection) => MovementDirection = movementDirection;

    public Vector2 SetRoamPosition(Vector2 newRoamPosition) => CurrentRoamPosition = newRoamPosition;
    public Transform GetCurrentTargetTransform() => _currentTargetTransform;

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
        IsSpawned = true;
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
        RoamPositionExistsCondition roamPositionExistsCondition = new RoamPositionExistsCondition(this);
        RoamTimerExpiredCondition roamTimerExpiredCondition = new RoamTimerExpiredCondition(this, RoamTimeMax);
        Inverter roamTimerExpiredConditionInverter = new Inverter(roamTimerExpiredCondition);
        MoveToRoamPositionNode moveToRoamPositionNode = new MoveToRoamPositionNode(this);
        Sequence roamSequence = new Sequence(new List<Node> { roamPositionExistsCondition, roamTimerExpiredConditionInverter, moveToRoamPositionNode });

        IdleToRoamNode idleToRoamNode = new IdleToRoamNode(this, TimeToStartRoamMin, TimeToStartRoamMax, RoamPositionOffsetMax);

        TargetInRangeCondition targetInAttackRangeCondition = new TargetInRangeCondition(this, AttackRangeMax);
        AttackOffCooldownCondition attackOffCooldownCondition = new AttackOffCooldownCondition(this);
        AttackNode attackNode = new AttackNode(this, AttackDelayMin, AttackDelayMax);
        Sequence attackSequnce = new Sequence(new List<Node> { targetInAttackRangeCondition, attackOffCooldownCondition, attackNode });

        TargetInChaseRangeCondition targetInChaseRangeCondition = new TargetInChaseRangeCondition(this, ChaseRange);
        ChaseNode chaseNode = new ChaseNode(this, AttackRangeMin, AttackRangeMax);
        Sequence chaseSequence = new Sequence(new List<Node> { targetInChaseRangeCondition, chaseNode });

        TargetExistsCondition targetExistsCondition = new TargetExistsCondition(this);
        Selector combatSelector = new Selector(new List<Node> { attackSequnce, chaseSequence });
        Sequence combatSequence = new Sequence(new List<Node> { targetExistsCondition, combatSelector });

        AgentSpawnedCondition agentSpawnedCondition = new AgentSpawnedCondition(this);
        Inverter agentSpawnedInverter = new Inverter(agentSpawnedCondition);
        Sequence enemySpawnSequence = new Sequence(new List<Node> { agentSpawnedInverter, idleToRoamNode });

        _behaviourTree = new Selector(new List<Node> { enemySpawnSequence, combatSequence, roamSequence, idleToRoamNode });
    }

    protected virtual void InitializeAgentTargetDetectionZone()
    {
        _targetDetectionZone = _enemyCore.GetAgentComponent<AgentTargetDetectionZone>();
        _targetDetectionZone.SetDetectionRadius(AggroRange);
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
