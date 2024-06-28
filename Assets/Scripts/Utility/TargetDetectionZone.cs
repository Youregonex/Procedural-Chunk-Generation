using UnityEngine;
using System;

[RequireComponent(typeof(CircleCollider2D))]
public class TargetDetectionZone : MonoBehaviour
{
    public event Action<Collider2D> OnTargetEnteredDetectionZone;
    public event Action<Collider2D> OnTargetLeftDetectionZone;

    [Header("Debug Fields")]
    [SerializeField] private CircleCollider2D _detectionZone;
    [SerializeField] private float _detectionRadius;

    private void Awake()
    {
        _detectionZone = GetComponent<CircleCollider2D>();

        _detectionZone.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTargetEnteredDetectionZone?.Invoke(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnTargetLeftDetectionZone?.Invoke(collision);
    }

    public void SetDetectionRadius(float newDetectionRadius)
    {
        _detectionRadius = newDetectionRadius;
        _detectionZone.radius = _detectionRadius;
    }
}
