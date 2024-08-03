using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class PreloadScreen : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float _loadTime = 1f;
    [SerializeField] private float _preloadScreenFadeTime = 1.5f;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(_loadTime);

        GetComponent<Image>().DOFade(0f, _preloadScreenFadeTime).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public void Initialize()
    {
        gameObject.SetActive(true);
    }
}
