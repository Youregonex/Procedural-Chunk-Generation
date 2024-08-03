using UnityEngine;
using UnityEngine.UI;
using Youregone.SceneLoader;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _exitButton;

    [Header("UI Components")]
    [SerializeField] private PreloadScreen _preloadScreen;

    private void Awake()
    {
        _preloadScreen.Initialize();

        _playButton.onClick.AddListener(() =>
        {
            SceneLoader.LoadScene(SceneLoader.ESceneName.GameScene);
        });

        _optionsButton.onClick.AddListener(() =>
        {

        });

        _exitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
