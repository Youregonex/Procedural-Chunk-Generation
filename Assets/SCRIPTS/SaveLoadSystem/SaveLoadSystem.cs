using UnityEngine;
using System;
using System.IO;

namespace Youregone.SaveLoadSystem
{
    public static class SaveLoadSystem
    {
        private static string _saveDirectory = $"/SaveFiles/";
        private static string _fileName = $"SaveGame.save";

        public static bool Save(GameData data)
        {
            string directory = Application.persistentDataPath + _saveDirectory;

            if(!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(directory + _fileName, json);

            Debug.Log("Saving Game");
            return true;
        }

        public static GameData Load()
        {
            string fullpath = Application.persistentDataPath + _saveDirectory + _fileName;

            GameData saveData = new GameData();

            if(File.Exists(fullpath))
            {
                string json = File.ReadAllText(fullpath);
                saveData = JsonUtility.FromJson<GameData>(json);
            }
            else
            {
                Debug.LogError("Savefile wasn't found!");
            }

            return saveData;
        }
    }
}