using UnityEngine;
using System;
using System.Collections;

public class AgentHealthSystem : AgentMonoBehaviourComponent, IContainLoot
{
    public event Action<DamageStruct> OnDamageTaken;
    public event EventHandler<OnHealthChangedEventArgs> OnHealthChanged;
    public class OnHealthChangedEventArgs : EventArgs
    {
        public float currentHealth;
        public float maxHealth;

        public OnHealthChangedEventArgs(float currentHealth, float maxHealth)
        {
            this.currentHealth = currentHealth;
            this.maxHealth = maxHealth;
        }
    }

    public event Action<AgentHealthSystem> OnDeath;
    public event Action OnLootDrop;

    [field: Header("Config")]
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _maxHealth;

    [SerializeField] protected float _destructionDelay = 0f;
    [SerializeField] protected Transform _damagePopupPosition;

    [Header("Debug Fields")]
    [SerializeField] protected bool _isDead = false;
    [SerializeField] protected AgentCoreBase _agentCore;
    [SerializeField] protected AgentAnimation _agentAnimation;

    public bool IsDead => _isDead;

    public float MaxHealth
    {
        get => _maxHealth;

        protected set
        {
            if(value < 1f)
            {
                _maxHealth = 1f;
                return;
            }

            _maxHealth = value;
        }
    }

    public float CurrentHealth
    {
        get => _currentHealth;

        protected set
        {
            if (value > _maxHealth)
            {
                _currentHealth = _maxHealth;
                return;
            }

            if (value < 0f)
            {
                _currentHealth = 0f;
                return;
            }

            _currentHealth = value;
        }
    }

    protected virtual void Awake()
    {
        CurrentHealth = MaxHealth;
        _agentCore = GetComponent<EnemyCore>();
    }

    protected virtual void Start()
    {
        _agentAnimation = _agentCore.GetAgentComponent<AgentAnimation>();
    }

    public void SetCurrentHealth(float currentHealth)
    {
        CurrentHealth = currentHealth;
        OnHealthChanged?.Invoke(this, new OnHealthChangedEventArgs(CurrentHealth, MaxHealth));
    }

    public void SetMaxHealth(float maxHealth)
    {
        MaxHealth = maxHealth;
        OnHealthChanged?.Invoke(this, new OnHealthChangedEventArgs(CurrentHealth, MaxHealth));
    }

    public override void DisableComponent()
    {
        this.enabled = false;
    }

    public override void EnableComponent()
    {
        this.enabled = true;
    }

    public void TakeDamage(DamageStruct damageStruct)
    {
        if (_isDead || _agentCore.GetFaction() == damageStruct.senderFaction)
            return;

        if(damageStruct.damageAmount < 0)
        {
            TakeHealing((int)damageStruct.damageAmount);
            return;
        }

        int damageTaken = CalculateDamage(damageStruct);

        _agentAnimation.ManageGetHitAnimation();
        CurrentHealth -= damageTaken;

        WorldTextDisplay.Instance.DisplayDamagePopup(_damagePopupPosition.position, damageTaken, Color.white);

        OnDamageTaken?.Invoke(damageStruct);

        if (CurrentHealth <= 0f)
        {
            OnDeath?.Invoke(this);
            Die();
        }

        OnHealthChanged?.Invoke(this, new OnHealthChangedEventArgs(CurrentHealth, MaxHealth));
    }

    public void TakeHealing(int healingAmount)
    {
        if(healingAmount < 0)
        {
            DamageStruct damageStruct = new DamageStruct(null, EFactions.None, healingAmount, 0f);
            TakeDamage(damageStruct);
            return;
        }

        CurrentHealth += healingAmount;

        OnHealthChanged?.Invoke(this, new OnHealthChangedEventArgs(CurrentHealth, MaxHealth));

        WorldTextDisplay.Instance.DisplayDamagePopup(_damagePopupPosition.position, healingAmount, Color.green);
    }

    protected int CalculateDamage(DamageStruct damageStruct)
    {
        int damageTaken = Mathf.RoundToInt(damageStruct.damageAmount);

        return damageTaken;
    }

    private void Die()
    {
        _isDead = true;

        _agentAnimation.PlayDeathAnimation();
        StartCoroutine(DestroyWithDelay());
    }

    private IEnumerator DestroyWithDelay()
    {
        yield return new WaitForSeconds(_destructionDelay);

        OnLootDrop?.Invoke();
        Destroy(gameObject);
    }
}