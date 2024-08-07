using UnityEngine;
using System;
using System.Collections;

public class AgentHealthSystem : AgentMonobehaviourComponent, IContainLoot
{
    public event Action<DamageStruct> OnDamageTaken;
    public event EventHandler<OnHealthChangedEventArgs> OnHealthChanged;
    public class OnHealthChangedEventArgs : EventArgs
    {
        public float currentHealth;
        public float maxHealth;
    }

    public event Action<AgentHealthSystem> OnDeath;
    public event Action OnLootDrop;

    [field: Header("Config")]
    [field: SerializeField] public float MaxHealth { get; protected set; }
    [field: SerializeField] public float CurrentHealth { get; protected set; }
    [SerializeField] protected float _destructionDelay = 0f;
    [SerializeField] protected Transform _damagePopupPosition;

    [Header("Debug Fields")]
    [SerializeField] protected bool _isDead = false;
    [SerializeField] protected AgentCoreBase _agentCore;
    [SerializeField] protected AgentAnimation _agentAnimation;
    [SerializeField] protected AgentHitbox _hitbox;

    public bool IsDead => _isDead;

    protected virtual void Awake()
    {
        CurrentHealth = MaxHealth;

        _agentCore = GetComponent<EnemyCore>();
    }

    protected virtual void Start()
    {
        _agentAnimation = _agentCore.GetAgentComponent<AgentAnimation>();
        _hitbox = _agentCore.GetAgentComponent<AgentHitbox>();
    }

    public void SetCurrentHealth(float currentHealth)
    {
        if (currentHealth > MaxHealth)
            currentHealth = MaxHealth;

        CurrentHealth = currentHealth;

        OnHealthChanged?.Invoke(this, new OnHealthChangedEventArgs
        {
            currentHealth = CurrentHealth,
            maxHealth = MaxHealth
        });
    }

    public void SetMaxHealth(float maxHealth)
    {
        MaxHealth = maxHealth;

        OnHealthChanged?.Invoke(this, new OnHealthChangedEventArgs
        {
            currentHealth = CurrentHealth,
            maxHealth = MaxHealth
        });
    }

    public override void DisableComponent()
    {
        this.enabled = false;
    }

    public override void EnableComponent()
    {
        this.enabled = true;
    }

    public virtual void TakeDamage(DamageStruct damageStruct)
    {
        if (_isDead || _agentCore.GetFaction() == damageStruct.senderFaction)
            return;

        int damageTaken = CalculateDamage(damageStruct);

        _agentAnimation.ManageGetHitAnimation();
        CurrentHealth -= damageTaken;

        WorldTextDisplay.Instance.DisplayDamagePopup(_damagePopupPosition.position, damageTaken);

        OnDamageTaken?.Invoke(damageStruct);

        if (CurrentHealth <= 0f)
            CurrentHealth = 0f;

        OnHealthChanged?.Invoke(this, new OnHealthChangedEventArgs
        {
            currentHealth = CurrentHealth,
            maxHealth = MaxHealth
        });

        if (CurrentHealth == 0f)
        {
            OnDeath?.Invoke(this);
            Die();
        }
    }

    public IDamageable GetHitbox() => _hitbox;

    protected int CalculateDamage(DamageStruct damageStruct)
    {
        int damageTaken = Mathf.RoundToInt(damageStruct.damageAmount);

        return damageTaken;
    }

    protected virtual void Die()
    {
        CurrentHealth = 0f;
        _isDead = true;
        _hitbox.enabled = false;

        _agentAnimation.PlayDeathAnimation();
        StartCoroutine(DestroyWithDelay());
    }

    protected IEnumerator DestroyWithDelay()
    {
        yield return new WaitForSeconds(_destructionDelay);

        OnLootDrop?.Invoke();
        Destroy(gameObject);
    }
}