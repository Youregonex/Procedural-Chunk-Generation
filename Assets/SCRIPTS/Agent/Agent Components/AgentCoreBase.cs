using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class AgentCoreBase : AgentMonobehaviourComponent
{
    [Header("Config")]
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
    [SerializeField] protected ItemHoldPoint _agentItemHoldPoint;
    [SerializeField] protected AgentAbilitySystem _agentAbilitySystem;

    [Header("Agent Colliders")]
    [SerializeField] protected CapsuleCollider2D _agentCollider;

    [Header("Agent RigidBody2D")]
    [SerializeField] protected Rigidbody2D _rigidBody2D;

    [Header("Debug Fields")]
    [SerializeField] protected List<AgentMonobehaviourComponent> _agentComponents = new List<AgentMonobehaviourComponent>();
    [SerializeField] protected List<AgentMonobehaviourComponent> _disableOnDeathComponents = new List<AgentMonobehaviourComponent>();

    public bool IsDead => _healthSystem.IsDead;

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

    public Rigidbody2D GetAgentRigidBody() => _rigidBody2D;
    public CapsuleCollider2D GetAgentCollider() => _agentCollider;
    public EFactions GetFaction() => _faction;

    public T GetAgentComponent<T>() where T : AgentMonobehaviourComponent
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


    public override void DisableComponent()
    {
        this.enabled = false;
    }

    public override void EnableComponent()
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
        foreach(AgentMonobehaviourComponent component in _agentComponents)
        {
            if (component != null && component.DisableOnDeath)
                _disableOnDeathComponents.Add(component);
        }
    }

    protected virtual void HealthSystem_OnDeath(AgentHealthSystem agentHealthSystem)
    {
        _agentCollider.enabled = false;

        foreach(AgentMonobehaviourComponent agentComponent in _disableOnDeathComponents)
        {
            agentComponent.DisableComponent();
        }
    }
}
