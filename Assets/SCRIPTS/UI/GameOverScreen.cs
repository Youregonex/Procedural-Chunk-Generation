using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [Header("Test")]
    [SerializeField] private Image _gameOverImage;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _reloadSceneButton;

    [SerializeField] private AgentHealthSystem _playerHealthSystem;

    private void Awake()
    {
        _exitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        _reloadSceneButton.onClick.AddListener(() =>
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        });
    }

    public void Initialize(AgentHealthSystem playerHealthSystem)
    {
        _playerHealthSystem = playerHealthSystem;

        _playerHealthSystem.OnDeath += AgentHealthSystem_OnDeath;
    }

    private void AgentHealthSystem_OnDeath(AgentHealthSystem obj)
    {
        _gameOverImage.gameObject.SetActive(true);
    }
}
