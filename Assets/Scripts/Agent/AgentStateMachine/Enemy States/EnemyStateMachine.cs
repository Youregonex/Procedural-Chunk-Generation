using UnityEngine;
using System.Collections.Generic;
using System;

public class EnemyStateMachine : BaseStateMachine<EnemyStateMachine.EEnemyState>
{
    public enum EEnemyState
    {
        Idle,
        Roam,
        Chase,
        Combat,
        Attack,
    }

    public event Action OnTargetInAttackRange;

    [Header("Config")]
    [SerializeField] private float _attackRangeMax;
    [SerializeField] private float _attackRangeMin;
    [SerializeField] private float _aggroRange;
    [SerializeField] private float _chaseRange;

    [SerializeField] private float _attackDelayMin = .1f;
    [SerializeField] private float _attackDelayMax = .3f;

    [SerializeField] private float _timeToStartRoamMax;
    [SerializeField] private Vector2 _roamPositionOffsetMax;

    [field: SerializeField] public bool DisableOnDeath { get; private set; }

    [Header("Debug Fields")]
    [SerializeField] private Vector2 _currentRoamPosition;

    [SerializeField] private Transform _currentTargetTransform;

    [field: SerializeField] public Vector2 MovementDirection { get; private set; }
    [field: SerializeField] public Vector2 AimPosition { get; private set; }

    [SerializeField] private AgentTargetDetectionZone _targetDetectionZone;
    [SerializeField] private AgentCoreBase _agentCore;

    [SerializeField] private bool _showGizmos;

    public List<Transform> TargetTransformList => _targetDetectionZone.TargetList;


    private void Awake()
    {
        _agentCore = GetComponent<AgentCoreBase>();

        InitializeStates();
    }

    protected override void Start()
    {
        base.Start();

        _targetDetectionZone = _agentCore.GetAgentComponent<AgentTargetDetectionZone>();

        _targetDetectionZone.SetDetectionRadius(_aggroRange);
    }

    protected override void Update()
    {
        PickCurrentTarget();

        base.Update();
    }

    public void SetAimPosition(Vector2 aimPosition) => AimPosition = aimPosition;
    public void SetMovementDirection(Vector2 movementDirection) => MovementDirection = movementDirection;

    public Vector2 GetPosition() => transform.position;

    public Vector2 GetCurrentRoamPosition() => _currentRoamPosition;
    public Vector2 SetCurrentRoamPosition(Vector2 newRoamPosition) => _currentRoamPosition = newRoamPosition;

    public void TriggerAttack()
    {
        OnTargetInAttackRange.Invoke();
    }

    public Transform GetCurrentTargetTransform() => _currentTargetTransform;

    protected override void InitializeStates()
    {
        _stateDictionary.Add(EEnemyState.Idle, new EnemyIdleState(EEnemyState.Idle, this, _timeToStartRoamMax, _roamPositionOffsetMax));
        _stateDictionary.Add(EEnemyState.Roam, new EnemyRoamState(EEnemyState.Roam, this));
        _stateDictionary.Add(EEnemyState.Chase, new EnemyChaseState(EEnemyState.Chase, this, _attackRangeMax));
        _stateDictionary.Add(EEnemyState.Combat, new EnemyCombatState(EEnemyState.Combat, this));
        _stateDictionary.Add(EEnemyState.Attack, new EnemyAttackState(EEnemyState.Attack, this, _attackDelayMin, _attackDelayMax, _attackRangeMax, _attackRangeMin));

        _currentState = _stateDictionary[EEnemyState.Idle];
    }

    private void PickCurrentTarget()
    {
        if(_currentTargetTransform == null || _currentTargetTransform.GetComponent<AgentHealthSystem>().IsDead)
            _currentTargetTransform = TargetTransformList.Count == 0 ? null : TargetTransformList[0];

        if (_currentTargetTransform != null && Vector2.Distance(transform.position, _currentTargetTransform.position) > _chaseRange)
            _currentTargetTransform = null;
    }

    private void OnDrawGizmos()
    {
        if (!_showGizmos)
            return;

        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(transform.position, _aggroRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRangeMax);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _attackRangeMin);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _chaseRange);
    }
}
