using UnityEngine;
using System.Collections;
using Youregone.SceneLoader;

public abstract class ScenePreloader : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] protected float _laodingScreenUnloadDelay = 0f;

    public abstract void StartPreloading();

    public virtual void FinishPreloading()
    {
        StartCoroutine(UnloadLoadingScreen());
    }

    private IEnumerator UnloadLoadingScreen()
    {
        yield return new WaitForSeconds(_laodingScreenUnloadDelay);

        SceneLoader.UnloadLoadingScene();
    }
}
