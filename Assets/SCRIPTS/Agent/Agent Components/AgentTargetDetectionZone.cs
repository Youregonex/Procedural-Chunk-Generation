using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CircleCollider2D))]
public class AgentTargetDetectionZone : AgentNetworkBehaviourComponent
{
    [Header("Config")]
    [SerializeField] private float _detectionInterval = .4f;

    [Header("Debug Fields")]
    [SerializeField] private CircleCollider2D _detectionZone;
    [SerializeField] private float _detectionRadius;
    [SerializeField] private float _detectionCooldown = 0f;
    [field: SerializeField] public List<Transform> TargetList { get; private set; } = new();

    private bool _isInitialized = false;

    public override void Initialize()
    {
        GetAgentCore();
        _detectionZone = GetComponent<CircleCollider2D>();
        _detectionZone.isTrigger = true;
    }

    public void SetDetectionZoneData(float newDetectionRadius)
    {
        SetDetectionRadius(newDetectionRadius);
        _isInitialized = true;
    }

    private void Update()
    {
        if (!_isInitialized)
            return;

        if (_detectionCooldown <= 0)
        {
            DetectTargets();
            _detectionCooldown = _detectionInterval;
        }
        else
            _detectionCooldown -= Time.deltaTime;
    }

    private void DetectTargets()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _detectionRadius);
        List<Transform> validTargets = new();

        foreach(Collider2D collider in colliders)
        {
            if(collider.TryGetComponent(out AgentCoreBase targetAgentCore) && targetAgentCore.Faction != AgentCore.Faction)
            {
                validTargets.Add(collider.transform);
            }
        }

        TargetList = validTargets;
    }

    private void SetDetectionRadius(float newDetectionRadius)
    {
        _detectionRadius = newDetectionRadius;
        _detectionZone.radius = _detectionRadius;
    }

    public override void DisableComponent()
    {
        _detectionZone.enabled = false;
        this.enabled = false;
    }

    public override void EnableComponent()
    {
        _detectionZone.enabled = true;
        this.enabled = true;
    }
}
