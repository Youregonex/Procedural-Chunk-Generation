using UnityEngine;
using UnityEngine.UI;

public class TestSaveLoad : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Button _saveButton;
    [SerializeField] private Button _loadButton;

    [Header("Debug Fields")]
    [SerializeField] private DataPersistanceManager _dataPersistanceManager;


    public void Initialize(DataPersistanceManager dataPersistanceManager)
    {
        _dataPersistanceManager = dataPersistanceManager;

        _saveButton.onClick.AddListener(() =>
        {
            _dataPersistanceManager.SaveGame();
        });

        _loadButton.onClick.AddListener(() =>
        {
            _dataPersistanceManager.LoadGame();
        });
    }
}
