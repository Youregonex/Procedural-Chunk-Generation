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

    [SerializeField] private float _attackDelayMin = .1f;
    [SerializeField] private float _attackDelayMax = .3f;

    [SerializeField] private float _timeToStartRoamMax;
    [SerializeField] private Vector2 _roamPositionOffsetMax;

    [SerializeField] private TargetDetectionZone _targetDetectionZone;

    [Header("Debug Fields")]
    [SerializeField] private Vector2 _currentRoamPosition;

    [SerializeField] private List<Transform> _targetTransformList = new List<Transform>();
    [SerializeField] private Transform _currentTargetTransform;
    [SerializeField] private HealthSystem _healthSystem;

    [field: SerializeField] public Vector2 MovementDirection { get; private set; }
    [field: SerializeField] public Vector2 AimPosition { get; private set; }

    [SerializeField] private bool _showGizmos;

    private void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>();

        InitializeStates();
    }

    protected override void Start()
    {
        base.Start();

        _targetDetectionZone.SetDetectionRadius(_aggroRange);

        _healthSystem.OnDamageTaken += HealthSystem_OnDamageTaken;
        _healthSystem.OnDeath += HealthSystem_OnDeath;
        _targetDetectionZone.OnTargetEnteredDetectionZone += TargetDetectionZone_OnTargetEnteredDetectionZone;
        _targetDetectionZone.OnTargetLeftDetectionZone += TargetDetectionZone_OnTargetLeftDetectionZone;
    }

    private void OnDestroy()
    {
        _healthSystem.OnDamageTaken -= HealthSystem_OnDamageTaken;
        _targetDetectionZone.OnTargetEnteredDetectionZone -= TargetDetectionZone_OnTargetEnteredDetectionZone;
        _targetDetectionZone.OnTargetLeftDetectionZone -= TargetDetectionZone_OnTargetLeftDetectionZone;
        _healthSystem.OnDeath -= HealthSystem_OnDeath;
    }

    public void SetAimPosition(Vector2 aimPosition) => AimPosition = aimPosition;
    public void SetMovementDirection(Vector2 movementDirection) => MovementDirection = movementDirection;

    public Vector2 GetPosition() => transform.position;
    public List<Transform> GetTargetTransformList() => _targetTransformList;

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

    private void HealthSystem_OnDeath()
    {
        this.enabled = false;
    }

    private void HealthSystem_OnDamageTaken(DamageStruct damageStruct)
    {
        //ChangeCurrentTarget(damageStruct.damageSender.transform);
    }

    private void TargetDetectionZone_OnTargetLeftDetectionZone(Collider2D collider)
    {
        if (!_targetTransformList.Contains(collider.transform))
            return;

        if (_targetTransformList.Count == 1)
        {
            ClearCurrentTarget();
            _targetTransformList.Remove(collider.transform);

            return;
        }

        if (_currentTargetTransform == collider.transform)
        {
            _targetTransformList.Remove(collider.transform);
            ChangeCurrentTarget(_targetTransformList[0]);
        }
        else
            _targetTransformList.Remove(collider.transform);
    }

    private void TargetDetectionZone_OnTargetEnteredDetectionZone(Collider2D collider)
    {
        AgentMovement agentMovement = collider.GetComponent<AgentMovement>();

        if (agentMovement == null)
            return;

        if (_targetTransformList.Count == 0)
        {
            ChangeCurrentTarget(agentMovement.transform);
        }

        _targetTransformList.Add(agentMovement.transform);
    }

    private void ChangeCurrentTarget(Transform newTarget)
    {
        if (newTarget == null)
        {
            ClearCurrentTarget();
            return;
        }

        HealthSystem targetHealthSystem = newTarget.GetComponent<HealthSystem>();
        targetHealthSystem.OnDeath += CurrentTarget_HealthSystem_OnDeath;

        if (_currentTargetTransform != null)
            _currentTargetTransform.GetComponent<HealthSystem>().OnDeath -= CurrentTarget_HealthSystem_OnDeath;

        _currentTargetTransform = newTarget;
    }

    private void ClearCurrentTarget()
    {
        if (_currentTargetTransform == null)
            return;

        HealthSystem targetHealthSystem = _currentTargetTransform.GetComponent<HealthSystem>();
        targetHealthSystem.OnDeath -= CurrentTarget_HealthSystem_OnDeath;
        _currentTargetTransform = null;
    }


    private void CurrentTarget_HealthSystem_OnDeath()
    {
        _targetTransformList.Remove(_currentTargetTransform);
        _currentTargetTransform = null;

        if (_targetTransformList.Count != 0)
            ChangeCurrentTarget(_targetTransformList[0]);
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
    }
}
