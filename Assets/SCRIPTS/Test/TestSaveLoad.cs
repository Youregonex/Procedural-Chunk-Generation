using UnityEngine;
using UnityEngine.UI;

public class TestSaveLoad : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Button _saveButton;
    [SerializeField] private Button _loadButton;

    public void Initialize()
    {
        _saveButton.onClick.AddListener(() =>
        {
            DataPersistanceManager.Instance.SaveGame();
        });

        _loadButton.onClick.AddListener(() =>
        {
            DataPersistanceManager.Instance.LoadGame();
        });
    }
}
