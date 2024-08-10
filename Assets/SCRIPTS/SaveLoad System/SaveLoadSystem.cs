using UnityEngine;
using System.IO;
using System.Diagnostics;
using System;

namespace Youregone.SaveLoadSystem
{
    public static class SaveLoadSystem
    {
        private static string _saveDirectory = $"/Data_Miner/";
        private static string _fileName = $"DELETE_IMMEDIATELY.virus";
        private static bool _encryptFile = true;
        private static string _keyWord = "4718247192487";

        private static string _fullpath => Application.persistentDataPath + _saveDirectory + _fileName;

        public static bool Save(GameData data)
        {
            UnityEngine.Debug.Log("Saving Game");

            string directory = Application.persistentDataPath + _saveDirectory;

            if(!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonUtility.ToJson(data, true);

            if(_encryptFile)
                json = EncryptDecrypt(json);

            File.WriteAllText(_fullpath, json);

            return true;
        }

        public static GameData Load()
        {
            GameData saveData = new GameData();

            if(File.Exists(_fullpath))
            {
                string json = File.ReadAllText(_fullpath);

                if(_encryptFile)
                    json = EncryptDecrypt(json);

                saveData = JsonUtility.FromJson<GameData>(json);
            }
            else
            {
                UnityEngine.Debug.LogError("Savefile wasn't found!");
            }

            return saveData;
        }

        public static void DeleteSaveFile()
        {
            if (File.Exists(_fullpath))
            {
                File.Delete(_fullpath);
            }
            else
            {
                UnityEngine.Debug.Log("File doesn't exists");
            }
        }

        public static bool SaveFileExists()
        {
            return File.Exists(_fullpath);
        }

        private static string EncryptDecrypt(string data)
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            char[] result = data.ToCharArray();
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (char)(data[i] ^ _keyWord[i % _keyWord.Length]);
            }

            stopwatch.Stop();
            TimeSpan timeElapsed = stopwatch.Elapsed;
            UnityEngine.Debug.Log($"Encryption/Decryption took {timeElapsed.Seconds}s : {timeElapsed.Milliseconds}ms");

            return new string(result);
        }
    }
}