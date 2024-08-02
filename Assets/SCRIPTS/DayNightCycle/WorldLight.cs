using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WorldLight : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private WorldTime _worldTime;
    [SerializeField] private Gradient _dayNightGradient;

    private const int MINUTES_IN_DAY = 1440;

    private Light2D _worldLight;

    private void Awake()
    {
        _worldLight = GetComponent<Light2D>();
    }

    private void Start()
    {
        _worldTime.OnTimeChanged += WorldTime_OnTimeChanged;
    }

    private void OnDestroy()
    {
        _worldTime.OnTimeChanged -= WorldTime_OnTimeChanged;
    }

    private void WorldTime_OnTimeChanged(TimeSpan currentTime)
    {
        _worldLight.color = _dayNightGradient.Evaluate(PercentOfDay(currentTime));
    }

    private float PercentOfDay(TimeSpan currentTime)
    {
        return (float)(currentTime.TotalMinutes % MINUTES_IN_DAY) / MINUTES_IN_DAY;
    }
}
