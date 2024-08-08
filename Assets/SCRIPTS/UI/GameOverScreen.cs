using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Youregone.SceneLoader;

public class GameOverScreen : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Image _gameOverImage;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private TextMeshProUGUI _gameOverText;

    [Header("Animation")]
    [SerializeField] private float _screenFadeInTime = 1.5f;

    [Header("Debug Fields")]
    [SerializeField] private AgentHealthSystem _playerHealthSystem;


    public void Initialize(AgentHealthSystem playerHealthSystem)
    {
        _playerHealthSystem = playerHealthSystem;
        _playerHealthSystem.OnDeath += AgentHealthSystem_OnDeath;
    }

    private void Awake()
    {
        _gameOverImage.gameObject.SetActive(false);
        _gameOverText.gameObject.SetActive(false);
        _mainMenuButton.gameObject.SetActive(false);

        _exitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        _mainMenuButton.onClick.AddListener(() =>
        {
            SceneLoader.LoadScene(SceneLoader.ESceneName.MainMenu);
        });
    }

    private void OnDestroy()
    {
        _playerHealthSystem.OnDeath -= AgentHealthSystem_OnDeath;
    }

    private void AgentHealthSystem_OnDeath(AgentHealthSystem obj)
    {
        _gameOverImage.color = new Color(0,0,0,0);
        _gameOverImage.gameObject.SetActive(true);

        _gameOverImage.DOFade(.8f, _screenFadeInTime).OnComplete(() =>
        {
            _gameOverText.gameObject.SetActive(true);
            _mainMenuButton.gameObject.SetActive(true);
        });
    }
}
