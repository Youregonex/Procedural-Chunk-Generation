using UnityEngine;
using System.Collections.Generic;
using System;

public class EnemyStateMachine : BaseStateMachine<BaseEnemyBehaviour.EBaseEnemyStates>
{
    public event Action OnTargetInAttackRange;

    [Header("Config")]
    [SerializeField] private float _attackRangeMax;
    [SerializeField] private float _attackRangeMin;
    [SerializeField] private float _aggroRange;
    [SerializeField] private float _chaseRange;
    [SerializeField] private float _combatRange;

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


}
