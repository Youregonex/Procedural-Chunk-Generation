using UnityEngine.SceneManagement;
using System;

namespace Youregone.SceneLoader
{
    public static class SceneLoader
    {
        private static Action _onLoaderCallback;

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
                SceneManager.LoadScene(sceneName.ToString());
            };

            SceneManager.LoadScene(ESceneName.Loading.ToString());
        }

        public static void SceneLoaderCallback()
        {
            if(_onLoaderCallback != null)
            {
                _onLoaderCallback();
                _onLoaderCallback = null;
            }
        }

    }
}
