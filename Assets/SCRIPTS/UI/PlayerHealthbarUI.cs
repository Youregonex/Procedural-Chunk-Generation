using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthbarUI : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Image _healthbarFill;
    [SerializeField] private TextMeshProUGUI _currentHealthText;
    [SerializeField] private TextMeshProUGUI _maxHealthText;

    [Header("Debug Fields")]
    [SerializeField] private AgentHealthSystem _playerHealthSystem;


    public void Initialize(AgentHealthSystem playerHealthSystem)
    {
        _playerHealthSystem = playerHealthSystem;

        _currentHealthText.text = _playerHealthSystem.CurrentHealth.ToString();
        _maxHealthText.text = _playerHealthSystem.MaxHealth.ToString();

        _playerHealthSystem.OnHealthChanged += AgentHealthSystem_OnHealthChanged;
    }

    private void OnDestroy()
    {
        _playerHealthSystem.OnHealthChanged -= AgentHealthSystem_OnHealthChanged;
    }

    private void AgentHealthSystem_OnHealthChanged(object sender, AgentHealthSystem.OnHealthChangedEventArgs e)
    {
        _healthbarFill.fillAmount = e.currentHealth / e.maxHealth;

        _currentHealthText.text = _playerHealthSystem.CurrentHealth.ToString();
        _maxHealthText.text = _playerHealthSystem.MaxHealth.ToString();
    }
}
