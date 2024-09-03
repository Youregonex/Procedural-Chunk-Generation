using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

public abstract class AgentCoreBase : NetworkBehaviour
{
    [Header("Config")]
    [SerializeField] private AgentItemHoldPoint _itemHoldPointPrefab;
    [SerializeField] protected EFactions _faction;

    [Header("Agent Components")]
    [SerializeField] protected AgentHealthSystem _healthSystem;
    [SerializeField] protected AgentAttackModule _agentAttackModule;
    [SerializeField] protected AgentMovement _agentMovement;
    [SerializeField] protected AgentAnimation _agentAnimation;
    [SerializeField] protected AgentVisual _agentVisual;
    [SerializeField] protected AgentHitbox _agentHitbox;
    [SerializeField] protected AgentInput _agentInput;
    [SerializeField] protected AgentStats _agentStats;
    [SerializeField] protected AgentAbilitySystem _agentAbilitySystem;

    [Header("Agent Colliders")]
    [SerializeField] protected CapsuleCollider2D _agentCollider;

    [Header("Agent RigidBody2D")]
    [SerializeField] protected Rigidbody2D _rigidBody;

    [field: Header("Debug Fields")]
    [field: SerializeField] public Transform SelfTransform { get; protected set; }
    [SerializeField] protected AgentItemHoldPoint _agentItemHoldPoint;
    [SerializeField] protected List<AgentNetworkBehaviourComponent> _agentComponents = new();
    [SerializeField] protected List<AgentNetworkBehaviourComponent> _disableOnDeathComponents = new();

    public Rigidbody2D AgentRigidBody => _rigidBody;
    public CapsuleCollider2D AgentCollider => _agentCollider;
    public EFactions Faction => _faction;
    public bool IsDead => _healthSystem.IsDead;

    public void Initialize()
    {
        _agentCollider = GetComponent<CapsuleCollider2D>();
        _rigidBody = GetComponent<Rigidbody2D>();

        SelfTransform = transform;
        InitializeComponentList();
        _healthSystem.OnDeath += HealthSystem_OnDeath;

        if(IsOwner)
            CreateItemHoldPointServerRpc(NetworkObject, NetworkObject.OwnerClientId);

        foreach(AgentNetworkBehaviourComponent agentComponent in _agentComponents)
        {
            agentComponent.Initialize();
        }
    }

    public override void OnNetworkSpawn()
    {
        Initialize();
    }

    [Rpc(SendTo.Server)]
    protected void CreateItemHoldPointServerRpc(NetworkObjectReference localPlayerCoreNetworkObjectReference, ulong localPlayerId)
    {
        AgentItemHoldPoint itemHoldPoint = Instantiate(_itemHoldPointPrefab);
        itemHoldPoint.NetworkObject.Spawn();

        if (localPlayerCoreNetworkObjectReference.TryGet(out NetworkObject localPlayerCoreNetworkObject))
        {
            itemHoldPoint.transform.SetParent(localPlayerCoreNetworkObject.transform);
            itemHoldPoint.transform.localPosition = Vector2.zero;
            itemHoldPoint.NetworkObject.ChangeOwnership(localPlayerId);
        }
        else
        {
            Debug.LogError("Couldn't unpack LocalPlayerTransformNetworkObjectReference");
        }

        AttachItemHoldPointClientRpc(itemHoldPoint.NetworkObject, localPlayerCoreNetworkObjectReference);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void AttachItemHoldPointClientRpc(NetworkObjectReference itemHoldPointNetworkObjectReference, NetworkObjectReference localPlayerCoreNetworkObjectReference)
    {
        if (itemHoldPointNetworkObjectReference.TryGet(out NetworkObject itemHoldPointNetworkObject) &&
            localPlayerCoreNetworkObjectReference.TryGet(out NetworkObject localPlayerCoreNetworkObject))
        {
            AgentCoreBase agentCore = localPlayerCoreNetworkObject.GetComponent<AgentCoreBase>();
            AgentItemHoldPoint itemHoldPoint = itemHoldPointNetworkObject.GetComponent<AgentItemHoldPoint>();
            agentCore.SetItemHoldPoint(itemHoldPoint);
            itemHoldPoint.Initialize();
        }
        else
        {
            Debug.LogError("Couldn't unpack ItemHoldPointNetworkObjectReference");
        }
    }

    public override void OnNetworkDespawn()
    {
        _healthSystem.OnDeath -= HealthSystem_OnDeath;
    }

    public void UpdateAgentItemHoldPoint()
    {
        AgentItemHoldPoint agentItemHoldPoint = transform.GetComponentInChildren<AgentItemHoldPoint>();
        SetItemHoldPoint(agentItemHoldPoint);
    }

    public void SetItemHoldPoint(AgentItemHoldPoint itemHoldPoint)
    {
        _agentItemHoldPoint = itemHoldPoint;

        if(!_agentComponents.Contains(_agentItemHoldPoint))
            _agentComponents.Add(_agentItemHoldPoint);

        if (_agentItemHoldPoint.DisableOnDeath && !_disableOnDeathComponents.Contains(_agentItemHoldPoint))
            _disableOnDeathComponents.Add(_agentItemHoldPoint);
    }

    public T GetAgentComponent<T>() where T : AgentNetworkBehaviourComponent
    {
        return _agentComponents.OfType<T>().FirstOrDefault();
    }

    public void DisableCollider()
    {
        _agentCollider.enabled = false;
    }

    public void EnableCollider()
    {
        _agentCollider.enabled = true;
    }

    public void DisableComponent()
    {
        this.enabled = false;
    }

    public void EnableComponent()
    {
        this.enabled = true;
    }

    protected virtual void InitializeComponentList()
    {
        _agentComponents.Add(_healthSystem);
        _agentComponents.Add(_agentAttackModule);
        _agentComponents.Add(_agentMovement);
        _agentComponents.Add(_agentAnimation);
        _agentComponents.Add(_agentVisual);
        _agentComponents.Add(_agentHitbox);
        _agentComponents.Add(_agentInput);
        _agentComponents.Add(_agentStats);
        _agentComponents.Add(_agentItemHoldPoint);
        _agentComponents.Add(_agentAbilitySystem);

        InitializeDisableOnDeathList();
    }

    protected virtual void InitializeDisableOnDeathList()
    {
        foreach(AgentNetworkBehaviourComponent component in _agentComponents)
        {
            if (component != null && component.DisableOnDeath)
                _disableOnDeathComponents.Add(component);
        }
    }

    protected virtual void HealthSystem_OnDeath(AgentHealthSystem agentHealthSystem)
    {
        DisableCollider();

        foreach (AgentNetworkBehaviourComponent agentComponent in _disableOnDeathComponents)
        {
            agentComponent.DisableComponent();
        }
    }
}
