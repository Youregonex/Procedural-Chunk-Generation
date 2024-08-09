using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Image _healthbarBackground;
    [SerializeField] private Image _healthbarFill;

    [Header("DebugFields")]
    [SerializeField] private AgentHealthSystem _healthSystem;

    private void Awake()
    {
        _healthSystem = transform.root.GetComponent<AgentHealthSystem>();
    }

    private void Start()
    {
        _healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        _healthSystem.OnDeath += HealthSystem_OnDeath;

        _healthbarFill.fillAmount = 1f;
        HideHealthbar();
    }

    private void OnDestroy()
    {
        _healthSystem.OnHealthChanged -= HealthSystem_OnHealthChanged;
        _healthSystem.OnDeath -= HealthSystem_OnDeath;
    }

    private void HealthSystem_OnDeath(AgentHealthSystem agentHealthSystem)
    {
        _healthbarFill.fillAmount = 0f;

        _healthbarBackground.enabled = false;
    }

    private void HealthSystem_OnHealthChanged(object sender, AgentHealthSystem.OnHealthChangedEventArgs e)
    {
        ShowHealthbar();

        float healthbarFill = e.currentHealth / e.maxHealth;

        _healthbarFill.fillAmount = healthbarFill;

        if (healthbarFill == 1)
            HideHealthbar();
    }

    private void ShowHealthbar()
    {
        _healthbarBackground.gameObject.SetActive(true);
    }

    private void HideHealthbar()
    {
        _healthbarBackground.gameObject.SetActive(false);
    }
}
