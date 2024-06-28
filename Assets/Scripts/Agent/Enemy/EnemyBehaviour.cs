using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class EnemyBehaviour : MonoBehaviour
{
    public event EventHandler OnTargetInAttackRange;

    [Header("Config")]
    [SerializeField] private float _attackRangeMax;
    [SerializeField] private float _attackRangeMin;
    [SerializeField] private float _aggroRange;
    [SerializeField] private float _timeToStartRoamMax;
    [SerializeField] private float _roamMaxTime;
    [SerializeField] private Vector2 _roamPositionOffsetMax;
    [SerializeField] private TargetDetectionZone _targetDetectionZone;

    [Header("Debug Fields")]
    [SerializeField] private float _attackDelayMin = .1f;
    [SerializeField] private float _attackDelayMax = .2f;
    [SerializeField] private float _attackDelayCurrent;

    [SerializeField] private float _timeToStartRoamCurrent;
    [SerializeField] private Vector2 _currentRoamPosition;
    [SerializeField] private float _roamCurrentTime;

    [SerializeField] private List<Transform> _targetTransformList = new List<Transform>();
    [SerializeField] private EnemyStateEnum _currentEnemyState = EnemyStateEnum.Idle;
    [SerializeField] private Transform _currentTargetTransform;

    [field: SerializeField] public Vector2 MovementDirection { get; private set; }
    [field: SerializeField] public Vector2 AimPosition { get; private set; }

    [SerializeField] private bool _showGizmos;

    private void Awake()
    {
        _attackDelayCurrent = UnityEngine.Random.Range(_attackDelayMin, _attackDelayMax);
    }

    private void Start()
    {
        _targetDetectionZone.SetDetectionRadius(_aggroRange);

        _targetDetectionZone.OnTargetEnteredDetectionZone += TargetDetectionZone_OnTargetEnteredDetectionZone;
        _targetDetectionZone.OnTargetLeftDetectionZone += TargetDetectionZone_OnTargetLeftDetectionZone;
    }

    private void Update()
    {
        switch(_currentEnemyState)
        {
            default:
            case EnemyStateEnum.Idle:

                ManageIdleState();
                break;

            case EnemyStateEnum.Chase:

                ManageChaseState();
                break;

            case EnemyStateEnum.Attack:

                ManageAttackState();
                break;

            case EnemyStateEnum.Roam:

                ManageRoamState();
                break;
        }
    }

    private void OnDestroy()
    {
        _targetDetectionZone.OnTargetEnteredDetectionZone -= TargetDetectionZone_OnTargetEnteredDetectionZone;
        _targetDetectionZone.OnTargetLeftDetectionZone -= TargetDetectionZone_OnTargetLeftDetectionZone;
    }

    private void ManageIdleState()
    {
        _currentRoamPosition = Vector2.zero;

        if (_timeToStartRoamCurrent > 0)
        {
            _timeToStartRoamCurrent -= Time.deltaTime;
        }
        else
        {
            _timeToStartRoamCurrent = _timeToStartRoamMax;

            _currentRoamPosition = PickRandomRoamPosition();
            _currentEnemyState = EnemyStateEnum.Roam;
        }

        MovementDirection = Vector3.zero;
    }

    private void ManageChaseState()
    {
        TargetExistscheck();

        float distanceToTarget = GetDistanceToTarget(_currentTargetTransform);

        MovementDirection = _currentTargetTransform.position - transform.position;
        AimPosition = _currentTargetTransform.transform.position;

        if (distanceToTarget <= _attackRangeMax)
            _currentEnemyState = EnemyStateEnum.Attack;
    }

    private void ManageAttackState()
    {
        TargetExistscheck();

        if(_attackDelayCurrent > 0)
        {
            _attackDelayCurrent -= Time.deltaTime;
            return;
        }

        _attackDelayCurrent = UnityEngine.Random.Range(_attackDelayMin, _attackDelayMax);

        MovementDirection = Vector3.zero;
        AimPosition = _currentTargetTransform.position;

        OnTargetInAttackRange?.Invoke(this, EventArgs.Empty);

        float distanceToTarget = GetDistanceToTarget(_currentTargetTransform);

        if (distanceToTarget > _attackRangeMax)
            _currentEnemyState = EnemyStateEnum.Chase;

        if (distanceToTarget < _attackRangeMin)
            FallbackFromTarget(_currentTargetTransform);
    }

    private void ManageRoamState()
    {
        if (_currentRoamPosition == Vector2.zero)
            return;

        if (_roamCurrentTime <= 0)
        {
            _roamCurrentTime = _roamMaxTime;
            _currentEnemyState = EnemyStateEnum.Idle;

            return;
        }
        else
            _roamCurrentTime -= Time.deltaTime;

        float closeToRoamPosition = .1f;

        if(Vector2.Distance(transform.position, _currentRoamPosition) >= closeToRoamPosition)
        {
            MovementDirection = _currentRoamPosition - (Vector2)transform.position;
            AimPosition = _currentRoamPosition;
        }
        else
        {
            _currentRoamPosition = Vector2.zero;
            _currentEnemyState = EnemyStateEnum.Idle;
        }
    }

    private void FallbackFromTarget(Transform target)
    {
        if (target == null)
            return;

        MovementDirection = transform.position - target.transform.position;
        AimPosition = _currentTargetTransform.position;
    }

    private void TargetExistscheck()
    {
        if (_currentTargetTransform == null)
        {
            if (_targetTransformList.Count > 0)
            {
                _currentTargetTransform = GetClosestTargetTransform();
            }
            else
            {
                _currentEnemyState = EnemyStateEnum.Idle;
                return;
            }
        }
    }

    private Transform GetClosestTargetTransform()
    {
        _targetTransformList = _targetTransformList.OrderBy(targetTransform => Vector3.Distance(transform.position, targetTransform.position)).ToList();

        return _targetTransformList[0];
    }

    private void TargetDetectionZone_OnTargetLeftDetectionZone(Collider2D collider)
    {
        if (!_targetTransformList.Contains(collider.transform))
            return;

        if (_targetTransformList.Count == 1)
        {
            _currentTargetTransform = null;
            _targetTransformList.Remove(collider.transform);
            _currentEnemyState = EnemyStateEnum.Idle;

            return;
        }

        if (_currentTargetTransform == collider.transform)
        {
            _targetTransformList.Remove(collider.transform);
            _currentTargetTransform = _targetTransformList[0];
        }
        else
            _targetTransformList.Remove(collider.transform);
    }

    private Vector2 PickRandomRoamPosition()
    {
        if (_currentRoamPosition != Vector2.zero)
            return _currentRoamPosition;

        float roamPositionX = UnityEngine.Random.Range(-_roamPositionOffsetMax.x, _roamPositionOffsetMax.x);
        float roamPositionY = UnityEngine.Random.Range(-_roamPositionOffsetMax.y, _roamPositionOffsetMax.y);

        Vector2 randomRoamPosition = new Vector2(roamPositionX, roamPositionY);

        return (Vector2)transform.position + randomRoamPosition;

    }

    private void TargetDetectionZone_OnTargetEnteredDetectionZone(Collider2D collider)
    {
        AgentMovement agentMovement = collider.GetComponent<AgentMovement>();

        if (agentMovement == null)
            return;

        if (_targetTransformList.Count == 0)
        {
            _currentTargetTransform = agentMovement.transform;
            _currentEnemyState = EnemyStateEnum.Chase;
        }

        _targetTransformList.Add(agentMovement.transform);
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

    private float GetDistanceToTarget(Transform target)
    {
        if (target == null)
            return 0;

        return Vector3.Distance(transform.position, target.position);
    }
}
