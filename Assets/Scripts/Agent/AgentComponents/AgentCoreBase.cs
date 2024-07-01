using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class AgentCoreBase : AgentMonobehaviourComponent
{
    [Header("Config")]
    [SerializeField] protected EFactions _faction;

    [Header("Agent Components")]
    [SerializeField] protected HealthSystem _healthSystem;
    [SerializeField] protected AgentAttackModule _agentAttackModule;
    [SerializeField] protected AgentMovement _agentMovement;
    [SerializeField] protected AgentAnimation _agentAnimation;
    [SerializeField] protected AgentVisual _agentVisual;
    [SerializeField] protected AgentHitbox _agentHitbox;
    [SerializeField] protected AgentInput _agentInput;
    [SerializeField] protected AgentStats _agentStats;

    [Header("Agent Colliders")]
    [SerializeField] private CapsuleCollider2D _collisionCollider;

    [Header("Agent RigidBody2D")]
    [SerializeField] private Rigidbody2D _rigidBody2D;

    [Header("Debug Fields")]
    [SerializeField] protected List<AgentMonobehaviourComponent> _agentComponents = new List<AgentMonobehaviourComponent>();
    [SerializeField] protected List<AgentMonobehaviourComponent> _disableOnDeathComponents = new List<AgentMonobehaviourComponent>();

    protected virtual void Awake()
    {
        InitializeComponentList();
    }

    protected virtual void Start()
    {
        _healthSystem.OnDeath += HealthSystem_OnDeath;
    }

    protected virtual void OnDestroy()
    {
        _healthSystem.OnDeath -= HealthSystem_OnDeath;
    }

    public Rigidbody2D GetAgentRigidBody2D() => _rigidBody2D;
    public CapsuleCollider2D GetAgentCollider() => _collisionCollider;
    public EFactions GetAgentFaction() => _faction;
    public AgentStats GetAgentStats() => _agentStats;

    public T GetAgentComponent<T>() where T : AgentMonobehaviourComponent
    {
        return _agentComponents.OfType<T>().FirstOrDefault();
    }

    public override void DisableComponent()
    {
        this.enabled = false;
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

        InitializeDisableOnDeathList();
    }

    protected virtual void InitializeDisableOnDeathList()
    {
        foreach(AgentMonobehaviourComponent component in _agentComponents)
        {
            if (component.DisableOnDeath)
                _disableOnDeathComponents.Add(component);
        }
    }

    protected virtual void HealthSystem_OnDeath()
    {
        foreach(AgentMonobehaviourComponent agentComponent in _disableOnDeathComponents)
        {
            agentComponent.DisableComponent();
        }
    }
}
