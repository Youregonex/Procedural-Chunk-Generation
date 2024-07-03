using UnityEngine;
using System.Collections.Generic;
using System;

public class BaseEnemyBehaviour : AgentMonobehaviourComponent
{
    public enum EBaseEnemyStates
    {
        Idle,
        Roam,
        Chase,
        Combat,
        Attack,
    }

    public event Action OnTargetInAttackRange;

    [Header("Config")]
    [SerializeField] protected BaseEnemyBehaviourDataSO _enemyBehaviourDataSO;

    [SerializeField] protected BaseEnemyIdleStateDataSO _idleStateDataSO;
    [SerializeField] protected BaseEnemyChaseStateDataSO _chaseStateDataSO;
    [SerializeField] protected BaseEnemyCombatStateDataSO _combatStateDataSO;
    [SerializeField] protected BaseEnemyRoamStateDataSO _roamStateDataSO;
    [SerializeField] protected BaseEnemyAttackStateDataSO _attackStateDataSO;

    [Header("Debug Fields")]
    [SerializeField] protected AgentCoreBase _agentCore;
    [SerializeField] private Vector2 _currentRoamPosition;
    [SerializeField] private Transform _currentTargetTransform;
    [field: SerializeField] public Vector2 MovementDirection { get; private set; }
    [field: SerializeField] public Vector2 AimPosition { get; private set; }
    [SerializeField] private AgentTargetDetectionZone _targetDetectionZone;
    [SerializeField] private bool _showGizmos;
    [SerializeField] protected BaseStateMachine<EBaseEnemyStates> _enemyStateMachine;

    public List<Transform> TargetTransformList => _targetDetectionZone.TargetList;

    public float AttackRangeMax { get; private set; }
    public float AttackRangeMin { get; private set; }
    public float AggroRange { get; private set; }
    public float ChaseRange { get; private set; }
    public float CombatRange { get; private set; }
    public float AttackDelayMin { get; private set; }
    public float AttackDelayMax { get; private set; }
    public float TimeToStartRoamMax { get; private set; }
    public Vector2 RoamPositionOffsetMax { get; private set; }

    protected virtual void Awake()
    {
        InitializeData();

        _agentCore = GetComponent<AgentCoreBase>();

        InitializeStateMachine();
    }

    protected virtual void Start()
    {
        InitializeAgentTargetDetectionZone();
    }

    protected virtual void Update()
    {
        PickCurrentTarget();

        _enemyStateMachine.Update();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _enemyStateMachine.OnTriggerEnter2D(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        _enemyStateMachine.OnTriggerStay2D(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _enemyStateMachine.OnTriggerExit2D(collision);
    }

    public void SetAimPosition(Vector2 aimPosition) => AimPosition = aimPosition;
    public void SetMovementDirection(Vector2 movementDirection) => MovementDirection = movementDirection;
    public Transform GetCurrentTargetTransform() => _currentTargetTransform;

    public override void DisableComponent()
    {
        this.enabled = false;
    }

    protected virtual void InitializeStateMachine()
    {
        _enemyStateMachine = new BaseStateMachine<EBaseEnemyStates>();

        _enemyStateMachine.InitializeState(EBaseEnemyStates.Idle, new EnemyIdleState(EBaseEnemyStates.Idle, this, _idleStateDataSO));
        _enemyStateMachine.InitializeState(EBaseEnemyStates.Roam, new EnemyRoamState(EBaseEnemyStates.Roam, this, _roamStateDataSO));
        _enemyStateMachine.InitializeState(EBaseEnemyStates.Chase, new EnemyChaseState(EBaseEnemyStates.Chase, this, _chaseStateDataSO));
        _enemyStateMachine.InitializeState(EBaseEnemyStates.Combat, new EnemyCombatState(EBaseEnemyStates.Combat, this, _combatStateDataSO));
        _enemyStateMachine.InitializeState(EBaseEnemyStates.Attack, new EnemyAttackState(EBaseEnemyStates.Attack, this, _attackStateDataSO));

        _enemyStateMachine.SetStartingState(EBaseEnemyStates.Idle);
    }

    protected virtual void InitializeData()
    {
        AttackRangeMax = _enemyBehaviourDataSO.AttackRangeMax;
        AttackRangeMin = _enemyBehaviourDataSO.AttackRangeMin;
        AggroRange = _enemyBehaviourDataSO.AggroRange;
        ChaseRange = _enemyBehaviourDataSO.ChaseRange;
        CombatRange = _enemyBehaviourDataSO.CombatRange;
        AttackDelayMin = _enemyBehaviourDataSO.AttackDelayMin;
        AttackDelayMax = _enemyBehaviourDataSO.AttackDelayMax;
        TimeToStartRoamMax = _enemyBehaviourDataSO.TimeToStartRoamMax;
        RoamPositionOffsetMax = _enemyBehaviourDataSO.RoamPositionOffsetMax;
    }

    private void PickCurrentTarget()
    {
        if (_currentTargetTransform == null || _currentTargetTransform.GetComponent<AgentHealthSystem>().IsDead)
            _currentTargetTransform = TargetTransformList.Count == 0 ? null : TargetTransformList[0];

        if (_currentTargetTransform != null && Vector2.Distance(transform.position, _currentTargetTransform.position) > ChaseRange)
            _currentTargetTransform = null;
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

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, ChaseRange);
    }
}
