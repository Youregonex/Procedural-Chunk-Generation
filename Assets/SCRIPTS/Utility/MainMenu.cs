using UnityEngine;
using UnityEngine.UI;
using Youregone.SceneLoader;
using Youregone.SaveLoadSystem;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _newGameButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _continueButton;

    [Header("Chunk Generator")]
    [SerializeField] private ChunkGenerator _chunkGenerator;

    private void Awake()
    {
        _chunkGenerator.StartGeneration();
    }

    private void Start()
    {
        _continueButton.onClick.AddListener(() =>
        {
            DataPersistanceManager.Instance.IsLoadingGame = true;
            SceneLoader.LoadScene(SceneLoader.ESceneName.GameScene);
        });

        _newGameButton.onClick.AddListener(() =>
        {
            DataPersistanceManager.Instance.IsLoadingGame = false;
            SceneLoader.LoadScene(SceneLoader.ESceneName.GameScene);
        });

        _optionsButton.onClick.AddListener(() =>
        {

        });

        _exitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        _continueButton.interactable = SaveLoadSystem.SaveFileExists();
    }
}
