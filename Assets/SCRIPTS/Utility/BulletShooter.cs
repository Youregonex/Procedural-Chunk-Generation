using UnityEngine;
using System.Collections;

public class BulletShooter
{
    //Projectile Settings
    private Projectile _prefab;
    private float _range;
    private float _damage;
    private float _moveSpeed;
    private float _startingDistance = .1f;

    //Burst Settings
    private int _burstCount;
    private int _projectilesPerBurst;
    private float _timeBetweenShots = .1f;
    private float _burstAngleSpread;
    private float _timeBetweenBursts;
    private float _restTime;
    private bool _updateTargetPositionEveryBurst = false;

    private bool _isShooting = false;
    private Transform _selfTransform;
    private MonoBehaviour _shooterMonoBehaviour;

    public BulletShooter(ShootPatternDataSO shootPatternDataSO, Transform shooterTransform, MonoBehaviour shooterMonoBehaviour)
    {
        _prefab = shootPatternDataSO.ProjectilePrefab;
        _range = shootPatternDataSO.ProjectileRange;
        _damage = shootPatternDataSO.ProjectileDamage;
        _moveSpeed = shootPatternDataSO.ProjectileSpeed;
        _startingDistance = shootPatternDataSO.StartingDistance;

        _burstCount = shootPatternDataSO.BurstCount;
        _projectilesPerBurst = shootPatternDataSO.ProjectilesPerBurst;
        _timeBetweenBursts = shootPatternDataSO.TimeBetweenBursts;
        _timeBetweenShots = shootPatternDataSO.TimeBetweenShots;
        _burstAngleSpread = shootPatternDataSO.BurstAngleSpread;
        _restTime = shootPatternDataSO.RestTime;
        _updateTargetPositionEveryBurst = shootPatternDataSO.UpdateTargetPositionEveryBurst;

        _selfTransform = shooterTransform;
        _shooterMonoBehaviour = shooterMonoBehaviour;
    }

    public void SpawnProjctiles(Transform targerTransform)
    {
        if (!_isShooting)
            _shooterMonoBehaviour.StartCoroutine(ShootCoroutine(targerTransform));
    }

    private IEnumerator ShootCoroutine(Transform targerTransform)
    {
        _isShooting = true;

        float startAngle;
        float currentAngle;
        float angleStep;

        CalculateTargetConeOfInfluence(targerTransform, out startAngle, out currentAngle, out angleStep);

        for (int i = 0; i < _burstCount; i++)
        {
            for (int j = 0; j < _projectilesPerBurst; j++)
            {
                Vector2 position = FindProjectileSpawnPosition(currentAngle);

                Projectile projectile = GameObject.Instantiate(_prefab, position, Quaternion.identity);
                projectile.transform.right = projectile.transform.position - _selfTransform.position;

                EFactions senderFaction = _selfTransform.GetComponent<AgentCoreBase>().GetFaction();
                DamageStruct damageStruct = new DamageStruct
                {
                    damageAmount = _damage,
                    damageSender = _selfTransform.gameObject,
                    senderFaction = senderFaction,
                    knockbackForce = 0f
                };

                projectile.SetupProjectile(_moveSpeed, _range, damageStruct);
                currentAngle += angleStep;

                if (_timeBetweenShots > 0)
                    yield return new WaitForSeconds(_timeBetweenShots);
            }

            currentAngle = startAngle;

            yield return new WaitForSeconds(_timeBetweenBursts);

            if(_updateTargetPositionEveryBurst)
                CalculateTargetConeOfInfluence(targerTransform, out startAngle, out currentAngle, out angleStep);
        }

        yield return new WaitForSeconds(_restTime);
        _isShooting = false;
    }

    private void CalculateTargetConeOfInfluence(Transform targerTransform, out float startAngle, out float currentAngle, out float angleStep)
    {
        Vector2 targetPosition;

        if (targerTransform == null)
            targetPosition = Vector2.right;
        else
            targetPosition = targerTransform.position;

        Vector2 targetDirection = targetPosition - (Vector2)_selfTransform.position;

        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        startAngle = targetAngle;
        float endAngle = targetAngle;
        currentAngle = targetAngle;
        float halfAngleSpread = 0f;
        angleStep = 0f;

        if (_burstAngleSpread != 0)
        {
            angleStep = _burstAngleSpread / (_projectilesPerBurst - 1);
            halfAngleSpread = _burstAngleSpread / 2f;
            startAngle = targetAngle - halfAngleSpread;
            endAngle = targetAngle + halfAngleSpread;
            currentAngle = startAngle;
        }
    }

    private Vector2 FindProjectileSpawnPosition(float currentAngle)
    {
        float x = _selfTransform.position.x + _startingDistance * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
        float y = _selfTransform.position.y + _startingDistance * Mathf.Sin(currentAngle * Mathf.Deg2Rad);

        Vector2 position = new Vector2(x, y);
        return position;
    }
}
