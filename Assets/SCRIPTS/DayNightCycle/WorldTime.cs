using UnityEngine;
using System;
using System.Collections;

public class WorldTime : MonoBehaviour
{
    public event Action<TimeSpan> OnTimeChanged;

    [SerializeField] private float _dayLength; // In seconds

    private const int MINUTES_IN_DAY = 1440;

    private TimeSpan _currentTime;
    private float _minuteLength => _dayLength / MINUTES_IN_DAY;

    private void Start()
    {
        StartCoroutine(AddMinute());
    }

    private IEnumerator AddMinute()
    {
        _currentTime += TimeSpan.FromMinutes(1);
        OnTimeChanged?.Invoke(_currentTime);
        yield return new WaitForSeconds(_minuteLength);

        StartCoroutine(AddMinute());
    }
}
