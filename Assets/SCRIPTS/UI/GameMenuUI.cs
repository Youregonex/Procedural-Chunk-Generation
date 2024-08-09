using UnityEngine;
using UnityEngine.UI;
using Youregone.SceneLoader;


public class GameMenuUI : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Image _background;
    [SerializeField] private Image _buttonsBackground;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _exitButton;

    [Header("Debug Fields")]
    [SerializeField] private bool _isEnabled = false;

    private void Awake()
    {
        _background.gameObject.SetActive(false);
        _buttonsBackground.gameObject.SetActive(false);
    }

    private void Start()
    {
        _continueButton.onClick.AddListener(() =>
        {
            HideMenuUI();
        });

        _optionsButton.onClick.AddListener(() =>
        {
            
        });

        _mainMenuButton.onClick.AddListener(() =>
        {
            DataPersistanceManager.Instance.SaveGame();

            SceneLoader.LoadScene(SceneLoader.ESceneName.MainMenu);
        });

        _exitButton.onClick.AddListener(() =>
        {
            DataPersistanceManager.Instance.SaveGame();

            Application.Quit();
        });
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ShowMenuUI();
        }
    }

    private void ShowMenuUI()
    {
        if(_isEnabled)
        {
            HideMenuUI();
            return;
        }

        _isEnabled = true;
        _background.gameObject.SetActive(true);
        _buttonsBackground.gameObject.SetActive(true);
    }

    private void HideMenuUI()
    {
        _isEnabled = false;
        _background.gameObject.SetActive(false);
        _buttonsBackground.gameObject.SetActive(false);
    }
}