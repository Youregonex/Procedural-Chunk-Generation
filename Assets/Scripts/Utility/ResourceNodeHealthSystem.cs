using System.Collections;
using UnityEngine;
using System;

public class ResourceNodeHealthSystem : MonoBehaviour
{
    public event Action<GatherStruct> OnDamageTaken;
    public event Action<int, int> OnHealthChanged;
    public event Action OnDeath;

    [Header("Config")]
    [SerializeField] private int _maxTicks;
    [SerializeField] private float _destructionDelay = 0;
    

    [Header("Debug Fields")]
    [SerializeField] private int _currentTicks;
    [SerializeField] private bool _isDepleted = false;

    public bool IsDepleted => _isDepleted;

    private void Awake()
    {
        _currentTicks = _maxTicks;
    }

    public void Gather(GatherStruct gatherStruct)
    {
        if (_isDepleted)
            return;

        _currentTicks -= gatherStruct.ticksPerHit;

        OnDamageTaken?.Invoke(gatherStruct);

        if (_currentTicks <= 0)
            _currentTicks = 0;

        OnHealthChanged?.Invoke(_currentTicks, _maxTicks);

        if (_currentTicks == 0)
        {
            OnDeath?.Invoke();

            DepleteNode();
        }
    }

    private void DepleteNode()
    {
        _isDepleted = true;

        StartCoroutine(DestroyWithDelay());
    }

    private IEnumerator DestroyWithDelay()
    {
        yield return new WaitForSeconds(_destructionDelay);

        Destroy(gameObject);
    }
}
