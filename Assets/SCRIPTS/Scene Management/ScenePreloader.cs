using UnityEngine;

public abstract class ScenePreloader : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] protected float _laodingScreenUnloadDelay = 0f;

    public abstract void StartPreloading(); 
}
