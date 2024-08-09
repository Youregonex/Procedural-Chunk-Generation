using UnityEngine;
using Youregone.SceneLoader;
using System.Collections;

public class GameScenePreloader : ScenePreloader
{
    [Header("Config")]
    [SerializeField] private GameSceneInitializer _sceneInitializer;

    public override void StartPreloading()
    {
        _sceneInitializer.SetupScene(this);
    }

    public void SetupComplete() // Call to unload Loading Scene
    {
        StartCoroutine(UnloadLoadingScreen());
    }

    private IEnumerator UnloadLoadingScreen()
    {
        yield return new WaitForSeconds(_laodingScreenUnloadDelay);

        SceneLoader.UnloadLoadingScene();
    }
}