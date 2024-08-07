using UnityEngine;

public class PlayerData : MonoBehaviour, IDataPersistance
{
    [Header("Debug Fields")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private PlayerCore _playerCore;

    private void Awake()
    {
        _playerCore = GetComponent<PlayerCore>();
        _playerTransform = _playerCore.SelfTransform;
    }

    public void LoadData(GameData gameData)
    {
        _playerTransform.position = gameData.playerPosition;
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.playerPosition = _playerTransform.position;
    }
}
