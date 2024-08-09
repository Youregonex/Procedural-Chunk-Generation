using UnityEngine.SceneManagement;
using System;
using UnityEngine;

namespace Youregone.SceneLoader
{
    public static class SceneLoader
    {
        public static event Action OnSceneLoaded;
        private static Action _onLoaderCallback;

        private const int LOADED_SCENE_INDEX = 1;

        public enum ESceneName
        {
            MainMenu,
            GameScene,
            Loading
        }

        public static void LoadScene(ESceneName sceneName)
        {
            _onLoaderCallback = () =>
            {
                SceneManager.LoadSceneAsync(sceneName.ToString(), LoadSceneMode.Additive);
                SceneManager.sceneLoaded += SceneManager_sceneLoaded;
                
            };

            SceneManager.LoadSceneAsync(ESceneName.Loading.ToString());
        }

        public static void UnloadLoadingScene()
        {
            SceneManager.UnloadSceneAsync(ESceneName.Loading.ToString());
        }

        public static void SceneLoaderCallback()
        {
            if (_onLoaderCallback != null)
            {
                _onLoaderCallback();
                _onLoaderCallback = null;
            }
        }

        private static void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
            SceneManager.SetActiveScene(SceneManager.GetSceneAt(LOADED_SCENE_INDEX));

            ScenePreloader scenePreloader = GameObject.FindObjectOfType<ScenePreloader>();
            scenePreloader.StartPreloading();

            OnSceneLoaded?.Invoke();
        }
    }
}