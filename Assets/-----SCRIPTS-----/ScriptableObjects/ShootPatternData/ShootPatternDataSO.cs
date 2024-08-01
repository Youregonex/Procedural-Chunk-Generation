using UnityEngine;

[CreateAssetMenu(menuName = "ShootPatternData")]
public class ShootPatternDataSO : ScriptableObject
{
    [field: Header("Projectile Settings")]
    [field: SerializeField] public Projectile ProjectilePrefab { get; private set; }
    [field: SerializeField] public float ProjectileRange { get; private set; }
    [field: SerializeField] public float ProjectileDamage { get; private set; }
    [field: SerializeField] public float ProjectileSpeed { get; private set; }
    [field: SerializeField, Range(.01f, .2f)] public float StartingDistance { get; private set; } = .1f;

    [field: Header("Burst Settings")]
    [field: SerializeField] public int BurstCount { get; private set; }
    [field: SerializeField] public int ProjectilesPerBurst { get; private set; }
    [field: SerializeField] public float TimeBetweenShots { get; private set; }
    [field: SerializeField, Range(0, 359)] public float BurstAngleSpread { get; private set; }
    [field: SerializeField] public float TimeBetweenBursts { get; private set; }
    [field: SerializeField] public float RestTime { get; private set; }
    [field: SerializeField] public bool UpdateTargetPositionEveryBurst { get; private set; }
}