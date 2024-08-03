using TMPro;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private TextMeshProUGUI _fpsText;

    private float _currentTime;
    private int _frameCount;
    private float _pollingTime = 1f;

    private void Update()
    {
        _currentTime += Time.deltaTime;
        _frameCount++;

        if(_currentTime >= _pollingTime)
        {
            int frameRate = Mathf.RoundToInt(_frameCount / _currentTime);
            _fpsText.text = $"{frameRate} FPS";

            _currentTime -= _pollingTime;
            _frameCount = 0;
        }
    }
}
