using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(CircleCollider2D))]
public class AgentTargetDetectionZone : AgentMonobehaviourComponent
{
    [Header("Config")]
    [SerializeField] private float _detectionInterval = .4f;

    [Header("Debug Fields")]
    [SerializeField] private CircleCollider2D _detectionZone;
    [SerializeField] private float _detectionRadius;
    [SerializeField] private float _detectionCooldown = 0f;
    [SerializeField] private EFactions _faction;
    [SerializeField] private AgentCoreBase _agentCore;
    [field: SerializeField] public List<Transform> TargetList { get; private set; } = new List<Transform>();


    private void Awake()
    {
        _detectionZone = GetComponent<CircleCollider2D>();
        _agentCore = transform.root.GetComponent<AgentCoreBase>();
        _detectionZone.isTrigger = true;
    }

    private void Start()
    {
        _faction = _agentCore.GetAgentFaction();
    }

    private void Update()
    {
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

        List<Transform> validTargets = new List<Transform>();

        foreach(Collider2D collider in colliders)
        {
            AgentCoreBase agentCore = collider.GetComponent<AgentCoreBase>();

            if (agentCore != null && agentCore.GetAgentFaction() != _agentCore.GetAgentFaction())
                validTargets.Add(collider.transform);
        }

        TargetList = validTargets;
    }

    public void SetDetectionRadius(float newDetectionRadius)
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
