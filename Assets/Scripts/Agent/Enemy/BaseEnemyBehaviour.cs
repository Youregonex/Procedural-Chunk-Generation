using UnityEngine;
using System.Collections.Generic;
using System;

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

    [Header("Debug Fields")]
    [SerializeField] private Node _behaviourTree;
    [SerializeField] private Transform _currentTargetTransform;
    [field: SerializeField] public bool IsSpawned { get; private set; }
    [field: SerializeField] public Vector2 CurrentRoamPosition { get; private set; }
    [field: SerializeField] public Vector2 MovementDirection { get; private set; }
    [field: SerializeField] public Vector2 AimPosition { get; private set; }
    [SerializeField] private AgentTargetDetectionZone _targetDetectionZone;
    [SerializeField] private AgentAttackModule _agentAttackModule;
    [SerializeField] protected AgentCoreBase _agentCore;
    [SerializeField] private bool _showGizmos;


    public List<Transform> TargetTransformList => _targetDetectionZone.TargetList;

    protected virtual void Awake()
    {
        _agentCore = GetComponent<AgentCoreBase>();

        ConstructBehaviourTree();
    }

    protected virtual void Start()
    {
        _agentAttackModule = _agentCore.GetAgentComponent<AgentAttackModule>();
        _agentCore.GetAgentComponent<AgentAnimation>().OnAgentSpawned += AgentAnimation_OnAgentSpawned;
        InitializeAgentTargetDetectionZone();
    }

    protected virtual void Update()
    {
        PickCurrentTarget();

        _behaviourTree.Evaluate();
    }

    protected void OnDestroy()
    {
        _agentCore.GetAgentComponent<AgentAnimation>().OnAgentSpawned -= AgentAnimation_OnAgentSpawned;
    }

    public void SetAimPosition(Vector2 aimPosition) => AimPosition = aimPosition;
    public void SetMovementDirection(Vector2 movementDirection) => MovementDirection = movementDirection;

    public Vector2 SetRoamPosition(Vector2 newRoamPosition) => CurrentRoamPosition = newRoamPosition;
    public Transform GetCurrentTargetTransform() => _currentTargetTransform;
    public float GetDistanceToCurrentTarget()
    {
        if (_currentTargetTransform == null)
            return 0;

        return Vector2.Distance(transform.position, _currentTargetTransform.position);
    }

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

    private void AgentAnimation_OnAgentSpawned()
    {
        IsSpawned = true;
    }

    private void PickCurrentTarget()
    {
        if (_currentTargetTransform == null || _currentTargetTransform.GetComponent<AgentHealthSystem>().IsDead)
            _currentTargetTransform = TargetTransformList.Count == 0 ? null : TargetTransformList[0];

        if (_currentTargetTransform != null && Vector2.Distance(transform.position, _currentTargetTransform.position) > ChaseRange)
            _currentTargetTransform = null;
    }

    private void ConstructBehaviourTree()
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

        TargetExistsCondition targetExistsCondition = new TargetExistsCondition(this);
        Selector combatSelector = new Selector(new List<Node> { attackSequnce, chaseSequence });
        Sequence combatSequence = new Sequence(new List<Node> { targetExistsCondition, combatSelector });

        AgentSpawnedCondition agentSpawnedCondition = new AgentSpawnedCondition(this);
        Inverter agentSpawnedInverter = new Inverter(agentSpawnedCondition);
        Sequence enemySpawnSequence = new Sequence(new List<Node> { agentSpawnedInverter, idleNode });

        Selector treeRoot = new Selector(new List<Node> { enemySpawnSequence, combatSequence, roamSequence, idleNode });

        _behaviourTree = treeRoot;
    }

    private void InitializeAgentTargetDetectionZone()
    {
        _targetDetectionZone = _agentCore.GetAgentComponent<AgentTargetDetectionZone>();
        _targetDetectionZone.SetDetectionRadius(AggroRange);
    }

    private void OnDrawGizmos()
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
