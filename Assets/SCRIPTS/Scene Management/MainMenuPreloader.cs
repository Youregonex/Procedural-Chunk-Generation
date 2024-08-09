using UnityEngine;
using Youregone.SceneLoader;
using System.Collections;

public class MainMenuPreloader : ScenePreloader
{
    public override void StartPreloading()
    {
        StartCoroutine(UnloadLoadingScreen());
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

    protected virtual void SceneLoader_OnSceneLoaded()
    {
        SceneLoader.OnSceneLoaded -= SceneLoader_OnSceneLoaded;

        StartCoroutine(UnloadLoadingScreen());
    }
}
