using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Youregone.SaveLoadSystem;

public class DataPersistanceManager : MonoBehaviour
{
    public static DataPersistanceManager Instance { get; private set; }

    private GameData _gameData = new GameData();
    private List<IDataPersistance> _dataPersistanceObjects;

    public bool IsLoadingGame;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void LoadGame()
    {
        FindDataPersistantObjects();
        _gameData = SaveLoadSystem.Load();

        foreach (IDataPersistance dataPersistanceObject in _dataPersistanceObjects)
        {
            dataPersistanceObject.LoadData(_gameData);
        }
    }

    public void SaveGame()
    {
        FindDataPersistantObjects();

        foreach (IDataPersistance dataPersistanceObject in _dataPersistanceObjects)
        {
            dataPersistanceObject.SaveData(ref _gameData);
        }

        SaveLoadSystem.Save(_gameData);
    }

    private void FindDataPersistantObjects()
    {
        IEnumerable<IDataPersistance> dataPersistancesObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistance>();

        _dataPersistanceObjects = new List<IDataPersistance>(dataPersistancesObjects);
    }
}