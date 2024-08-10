using UnityEngine;

public class GameScenePreloader : ScenePreloader
{
    [Header("Config")]
    [SerializeField] private GameSceneInitializer _sceneInitializer;

    public override void StartPreloading()
    {
        _sceneInitializer.SetupScene(this);
    }
}